using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;

namespace Nop.Plugin.Widgets.BlankTable.Services.Catalog
{
    /// <summary>
    /// Product attribute parser
    /// </summary>
    public partial class ProductAttributeParser : BaseAttributeParser, IProductAttributeParser
    {

        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ProductAttributeParser(ICurrencyService currencyService,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            IProductAttributeService productAttributeService,
            IRepository<ProductAttributeValue> productAttributeValueRepository,
            IWorkContext workContext)
        {
            _currencyService = currencyService;
            _downloadService = downloadService;
            _productAttributeService = productAttributeService;
            _productAttributeValueRepository = productAttributeValueRepository;
            _workContext = workContext;
            _localizationService = localizationService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Returns a list which contains all possible combinations of elements
        /// </summary>
        /// <typeparam name="T">Type of element</typeparam>
        /// <param name="elements">Elements to make combinations</param>
        /// <returns>All possible combinations of elements</returns>
        protected virtual IList<IList<T>> CreateCombination<T>(IList<T> elements)
        {
            var rez = new List<IList<T>>();

            for (var i = 1; i < Math.Pow(2, elements.Count); i++)
            {
                var current = new List<T>();
                var index = -1;

                //transform int to binary string
                var binaryMask = Convert.ToString(i, 2).PadLeft(elements.Count, '0');

                foreach (var flag in binaryMask)
                {
                    index++;

                    if (flag == '0')
                        continue;

                    //add element if binary mask in the position of element has 1
                    current.Add(elements[index]);
                }

                rez.Add(current);
            }

            return rez;
        }
        
        /// <summary>
        /// Gets selected product attribute values with the quantity entered by the customer
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="productAttributeMappingId">Product attribute mapping identifier</param>
        /// <returns>Collections of pairs of product attribute values and their quantity</returns>
        protected IList<Tuple<string, string>> ParseValuesWithQuantity(string attributesXml, int productAttributeMappingId)
        {
            var selectedValues = new List<Tuple<string, string>>();
            if (string.IsNullOrEmpty(attributesXml))
                return selectedValues;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                foreach (XmlNode attributeNode in xmlDoc.SelectNodes(@"//Attributes/ProductAttribute"))
                {
                    if (attributeNode.Attributes?["ID"] == null)
                        continue;

                    if (!int.TryParse(attributeNode.Attributes["ID"].InnerText.Trim(), out var attributeId) ||
                        attributeId != productAttributeMappingId)
                        continue;

                    foreach (XmlNode attributeValue in attributeNode.SelectNodes("ProductAttributeValue"))
                    {
                        var value = attributeValue.SelectSingleNode("Value").InnerText.Trim();
                        var quantityNode = attributeValue.SelectSingleNode("Quantity");
                        selectedValues.Add(new Tuple<string, string>(value, quantityNode != null ? quantityNode.InnerText.Trim() : string.Empty));
                    }
                }
            }
            catch
            {
                // ignored
            }

            return selectedValues;
        }

        /// <summary>
        /// Adds gift cards attributes in XML format
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        protected virtual void AddGiftCardsAttributesXml(Product product, IFormCollection form, ref string attributesXml)
        {
            if (!product.IsGiftCard)
                return;

            var recipientName = "";
            var recipientEmail = "";
            var senderName = "";
            var senderEmail = "";
            var giftCardMessage = "";
            foreach (var formKey in form.Keys)
            {
                if (formKey.Equals($"giftcard_{product.Id}.RecipientName", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientName = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.RecipientEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientEmail = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.SenderName", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderName = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.SenderEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderEmail = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.Message", StringComparison.InvariantCultureIgnoreCase))
                {
                    giftCardMessage = form[formKey];
                }
            }

            attributesXml = AddGiftCardAttribute(attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
        }

        #endregion

        #region Product attributes

        /// <summary>
        /// Gets selected product attribute mappings
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the selected product attribute mappings
        /// </returns>
        public virtual async Task<IList<ProductAttributeMapping>> ParseProductAttributeMappingsAsync(string attributesXml)
        {
            var result = new List<ProductAttributeMapping>();
            if (string.IsNullOrEmpty(attributesXml))
                return result;

            var ids = ParseAttributeIds(attributesXml);
            foreach (var id in ids)
            {
                var attribute = await _productAttributeService.GetProductAttributeMappingByIdAsync(id);
                if (attribute != null) 
                    result.Add(attribute);
            }

            return result;
        }

        /// <summary>
        /// Gets selected product attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="productAttributeMappingId">Product attribute mapping identifier</param>
        /// <returns>Product attribute values</returns>
        public virtual IList<string> ParseValues(string attributesXml, int productAttributeMappingId)
        {
            var selectedValues = new List<string>();
            if (string.IsNullOrEmpty(attributesXml))
                return selectedValues;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/ProductAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["ID"] == null)
                        continue;

                    var str1 = node1.Attributes["ID"].InnerText.Trim();
                    if (!int.TryParse(str1, out var id))
                        continue;

                    if (id != productAttributeMappingId)
                        continue;

                    var nodeList2 = node1.SelectNodes(@"ProductAttributeValue/Value");
                    foreach (XmlNode node2 in nodeList2)
                    {
                        var value = node2.InnerText.Trim();
                        selectedValues.Add(value);
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return selectedValues;
        }

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <param name="value">Value</param>
        /// <param name="quantity">Quantity (used with AttributeValueType.AssociatedToProduct to specify the quantity entered by the customer)</param>
        /// <returns>Updated result (XML format)</returns>
        public virtual string AddProductAttribute(string attributesXml, ProductAttributeMapping productAttributeMapping, string value, int? quantity = null)
        {
            var result = string.Empty;
            try
            {
                var xmlDoc = new XmlDocument();
                if (string.IsNullOrEmpty(attributesXml))
                {
                    var element1 = xmlDoc.CreateElement("Attributes");
                    xmlDoc.AppendChild(element1);
                }
                else
                {
                    xmlDoc.LoadXml(attributesXml);
                }

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");

                XmlElement attributeElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/ProductAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["ID"] == null)
                        continue;

                    var str1 = node1.Attributes["ID"].InnerText.Trim();
                    if (!int.TryParse(str1, out var id))
                        continue;

                    if (id != productAttributeMapping.Id)
                        continue;

                    attributeElement = (XmlElement)node1;
                    break;
                }

                //create new one if not found
                if (attributeElement == null)
                {
                    attributeElement = xmlDoc.CreateElement("ProductAttribute");
                    attributeElement.SetAttribute("ID", productAttributeMapping.Id.ToString());
                    rootElement.AppendChild(attributeElement);
                }

                var attributeValueElement = xmlDoc.CreateElement("ProductAttributeValue");
                attributeElement.AppendChild(attributeValueElement);

                var attributeValueValueElement = xmlDoc.CreateElement("Value");
                attributeValueValueElement.InnerText = value;
                attributeValueElement.AppendChild(attributeValueValueElement);

                //the quantity entered by the customer
                if (quantity.HasValue)
                {
                    var attributeValueQuantity = xmlDoc.CreateElement("Quantity");
                    attributeValueQuantity.InnerText = quantity.ToString();
                    attributeValueElement.AppendChild(attributeValueQuantity);
                }

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return result;
        }

        /// <summary>
        /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
        /// </summary>
        /// <param name="pam">Product attribute</param>
        /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool?> IsConditionMetAsync(ProductAttributeMapping pam, string selectedAttributesXml)
        {
            if (pam == null)
                throw new ArgumentNullException(nameof(pam));

            var conditionAttributeXml = pam.ConditionAttributeXml;
            if (string.IsNullOrEmpty(conditionAttributeXml))
                //no condition
                return null;

            //load an attribute this one depends on
            var dependOnAttribute = (await ParseProductAttributeMappingsAsync(conditionAttributeXml)).FirstOrDefault();
            if (dependOnAttribute == null)
                return true;

            var valuesThatShouldBeSelected = ParseValues(conditionAttributeXml, dependOnAttribute.Id)
                //a workaround here:
                //ConditionAttributeXml can contain "empty" values (nothing is selected)
                //but in other cases (like below) we do not store empty values
                //that's why we remove empty values here
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
            var selectedValues = ParseValues(selectedAttributesXml, dependOnAttribute.Id);
            if (valuesThatShouldBeSelected.Count != selectedValues.Count)
                return false;

            //compare values
            var allFound = true;
            foreach (var t1 in valuesThatShouldBeSelected)
            {
                var found = false;
                foreach (var t2 in selectedValues)
                    if (t1 == t2)
                        found = true;
                if (!found)
                    allFound = false;
            }

            return allFound;
        }

        /// <summary>
        /// Parse a customer entered price of the product
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer entered price of the product
        /// </returns>
        public virtual async Task<decimal> ParseCustomerEnteredPriceAsync(Product product, IFormCollection form)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var customerEnteredPriceConverted = decimal.Zero;
            if (product.CustomerEntersPrice)
                foreach (var formKey in form.Keys)
                {
                    if (formKey.Equals($"addtocart_{product.Id}.CustomerEnteredPrice", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (decimal.TryParse(form[formKey], out var customerEnteredPrice))
                            customerEnteredPriceConverted = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(customerEnteredPrice, await _workContext.GetWorkingCurrencyAsync());
                        break;
                    }
                }

            return customerEnteredPriceConverted;
        }

        /// <summary>
        /// Parse a entered quantity of the product
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <returns>Customer entered price of the product</returns>
        public virtual int ParseEnteredQuantity(Product product, IFormCollection form)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var quantity = 1;
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"addtocart_{product.Id}.EnteredQuantity", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(form[formKey], out quantity);
                    break;
                }

            return quantity;
        }

        /// <summary>
        /// Parse product rental dates on the product details page
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        public virtual void ParseRentalDates(Product product, IFormCollection form, out DateTime? startDate, out DateTime? endDate)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            startDate = null;
            endDate = null;

            if (product.IsRental)
            {
                var ctrlStartDate = form[$"rental_start_date_{product.Id}"];
                var ctrlEndDate = form[$"rental_end_date_{product.Id}"];
                try
                {
                    startDate = DateTime.ParseExact(ctrlStartDate, 
                        CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, 
                        CultureInfo.InvariantCulture);
                    endDate = DateTime.ParseExact(ctrlEndDate, 
                        CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, 
                        CultureInfo.InvariantCulture);
                }
                catch
                {
                    // ignored
                }
            }
        }

        #endregion

        #region Gift card attributes

        /// <summary>
        /// Add gift card attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="recipientName">Recipient name</param>
        /// <param name="recipientEmail">Recipient email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="giftCardMessage">Message</param>
        /// <returns>Attributes</returns>
        public string AddGiftCardAttribute(string attributesXml, string recipientName,
            string recipientEmail, string senderName, string senderEmail, string giftCardMessage)
        {
            var result = string.Empty;
            try
            {
                recipientName = recipientName.Trim();
                recipientEmail = recipientEmail.Trim();
                senderName = senderName.Trim();
                senderEmail = senderEmail.Trim();

                var xmlDoc = new XmlDocument();
                if (string.IsNullOrEmpty(attributesXml))
                {
                    var element1 = xmlDoc.CreateElement("Attributes");
                    xmlDoc.AppendChild(element1);
                }
                else
                    xmlDoc.LoadXml(attributesXml);

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");

                var giftCardElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo");
                if (giftCardElement == null)
                {
                    giftCardElement = xmlDoc.CreateElement("GiftCardInfo");
                    rootElement.AppendChild(giftCardElement);
                }

                var recipientNameElement = xmlDoc.CreateElement("RecipientName");
                recipientNameElement.InnerText = recipientName;
                giftCardElement.AppendChild(recipientNameElement);

                var recipientEmailElement = xmlDoc.CreateElement("RecipientEmail");
                recipientEmailElement.InnerText = recipientEmail;
                giftCardElement.AppendChild(recipientEmailElement);

                var senderNameElement = xmlDoc.CreateElement("SenderName");
                senderNameElement.InnerText = senderName;
                giftCardElement.AppendChild(senderNameElement);

                var senderEmailElement = xmlDoc.CreateElement("SenderEmail");
                senderEmailElement.InnerText = senderEmail;
                giftCardElement.AppendChild(senderEmailElement);

                var messageElement = xmlDoc.CreateElement("Message");
                messageElement.InnerText = giftCardMessage;
                giftCardElement.AppendChild(messageElement);

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return result;
        }

        /// <summary>
        /// Get gift card attributes
        /// </summary>
        /// <param name="attributesXml">Attributes</param>
        /// <param name="recipientName">Recipient name</param>
        /// <param name="recipientEmail">Recipient email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="giftCardMessage">Message</param>
        public void GetGiftCardAttribute(string attributesXml, out string recipientName,
            out string recipientEmail, out string senderName,
            out string senderEmail, out string giftCardMessage)
        {
            recipientName = string.Empty;
            recipientEmail = string.Empty;
            senderName = string.Empty;
            senderEmail = string.Empty;
            giftCardMessage = string.Empty;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                var recipientNameElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/RecipientName");
                var recipientEmailElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/RecipientEmail");
                var senderNameElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/SenderName");
                var senderEmailElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/SenderEmail");
                var messageElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/Message");

                if (recipientNameElement != null)
                    recipientName = recipientNameElement.InnerText;
                if (recipientEmailElement != null)
                    recipientEmail = recipientEmailElement.InnerText;
                if (senderNameElement != null)
                    senderName = senderNameElement.InnerText;
                if (senderEmailElement != null)
                    senderEmail = senderEmailElement.InnerText;
                if (messageElement != null)
                    giftCardMessage = messageElement.InnerText;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
        }

        #endregion

        #region Properties

        protected override string RootElementName { get; set; } = "Attributes";

        protected override string ChildElementName { get; set; } = "ProductAttribute";

        #endregion
    }
}