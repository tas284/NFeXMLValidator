using NFeXMLValidator.Interfaces;
using System.Xml;
using System.Xml.Schema;

namespace NFeXMLValidator.Services
{
    public class XMLValidator : IXMLValidator
    {
        private static readonly ICollection<string> fails = new List<string>();

        public string ValidateNFe(string xml)
        {
            try
            {
                XmlDocument doc = LoadXmlDocument(xml);

                string fails = ExecuteValidation(doc);

                if (fails.Count() > 0)
                    return fails;
                else
                    return "Arquivo validado com sucesso!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private XmlDocument LoadXmlDocument(string xml)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
                if (doc.FirstChild!.Name == "nfeProc" && doc.FirstChild.FirstChild!.Name == "NFe")
                    doc.LoadXml(doc.FirstChild.FirstChild.OuterXml);

                if (doc.FirstChild!.Name != "NFe" || !doc.FirstChild.HasChildNodes || doc.FirstChild.InnerXml.Length < 2000) throw new Exception();
            }
            catch(Exception ex)
            {
                throw new XmlException("Arquivo XML inválido, informe outro arquivo.", ex.InnerException);
            }
            return doc;
        }
        private string ExecuteValidation(XmlDocument document)
        {
            string result = string.Empty;

            ICollection<string> XSDFiles = new List<string>();
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    XSDFiles.Add(@"Schemas/procNFe_v4.00.xsd");
                    XSDFiles.Add(@"Schemas/nfe_v4.00.xsd");
                    XSDFiles.Add(@"Schemas/leiauteNFe_v4.00.xsd");
                    XSDFiles.Add(@"Schemas/tiposBasico_v4.00.xsd");
                    XSDFiles.Add(@"Schemas/xmldsig-core-schema_v1.01.xsd");
                }
                else
                {
                    XSDFiles.Add(@"Schemas\procNFe_v4.00.xsd");
                    XSDFiles.Add(@"Schemas\nfe_v4.00.xsd");
                    XSDFiles.Add(@"Schemas\leiauteNFe_v4.00.xsd");
                    XSDFiles.Add(@"Schemas\tiposBasico_v4.00.xsd");
                    XSDFiles.Add(@"Schemas\xmldsig-core-schema_v1.01.xsd");
                }

            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(ex.Message, ex.InnerException);
            }

            List<string> validations = ValidateDocument(document, XSDFiles).ToList();

            if (validations.Count() > 0)
            {
                result = $"Validação do XML:";
                foreach (var validation in validations)
                {
                    result += validation;
                }
            }

            return result;
        }
        private static ICollection<string> ValidateDocument(XmlDocument doc, ICollection<string> XSDFiles)
        {
            fails.Clear();
            try
            {
                foreach (var xsd in XSDFiles)
                {
                    doc.Schemas.Add(null, xsd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Houve um erro ao incluir os arquivos XSD para validar o arquivo XML. {ex.Message}");
            }

            try
            {
                doc.Validate(ValidationCallback!);
            }
            catch (XmlSchemaValidationException ex)
            {
                throw new Exception($"Houve um erro ao executar a validação do documento XML. {ex.Message}");
            }

            return fails;
        }
        private static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
            {
                fails.Add(" Alerta: " + ConvertErrorMessages(args.Message) + " (Caminho: " + GetPath(args) + ")");
            }
            if (args.Severity == XmlSeverityType.Error)
            {
                fails.Add(" Erro: " + ConvertErrorMessages(args.Message) + " (Caminho: " + GetPath(args) + ")");
            }
        }
        private static string ConvertErrorMessages(string message)
        {
            message = message.Replace("The value of the 'Algorithm' attribute does not equal its fixed value.", "O valor do atributo 'Algorithm' não é igual ao seu valor fixo.");
            message = message.Replace("The '", "O elemento '");
            message = message.Replace("element is invalid", "é inválido");
            message = message.Replace("The value", "O valor");
            message = message.Replace("is invalid according to its datatype", "é inválido de acordo com o seu tipo de dados");
            message = message.Replace("The Pattern constraint failed.", "");
            message = message.Replace("The actual length is less than the MinLength value", "O comprimento real é menor que o valor MinLength");
            message = message.Replace(" in namespace 'http://www.w3.org/2000/09/xmldsig#'.", "");
            message = message.Replace(" in namespace 'http://www.portalfiscal.inf.br/nfe'.", "");
            message = message.Replace(" in namespace 'http://www.portalfiscal.inf.br/nfe'", "");
            message = message.Replace("http://www.portalfiscal.inf.br/nfe:", "");
            message = message.Replace("The element", "O elemento");
            message = message.Replace("has invalid child element", "tem um elemento filho inválido");
            message = message.Replace("List of possible elements expected:", "Lista de possíveis elementos esperados:");
            message = message.Replace("The Enumeration constraint failed.", "");
            message = message.Replace("http://www.w3.org/2000/09/xmldsig#:", "");
            message = message.Replace("http://www.w3.org/2001/XMLSchema:", "");
            message = message.Replace("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters.", "A entrada não é uma string Base-64 válida, pois contém um caractere não base 64, mais de dois caracteres de preenchimento ou um caractere ilegal entre os caracteres de preenchimento.");
            message = message.Replace("The required attribute", "O atributo obrigatório");
            message = message.Replace("is missing", "está ausente");
            message = message.Replace("has incomplete content", "tem conteúdo incompleto");
            message = message.Replace("as well as", "bem como");
            return message;
        }
        private static string GetPath(ValidationEventArgs args)
        {
            XmlSchemaValidationException ex = (XmlSchemaValidationException)args.Exception;
            object sourceObject = ex.SourceObject!;

            if (sourceObject!.GetType() == typeof(XmlElement))
            {
                XmlElement tag = (XmlElement)sourceObject;
                return GetXMLTree(tag.ParentNode!) + "/" + tag.Name;
            }
            else
            {
                return "";
            }
        }
        private static string GetXMLTree(XmlNode args)
        {
            var node = args.ParentNode;
            if (args.ParentNode == null)
            {
                return "";
            }
            else if (args.ParentNode.NodeType == XmlNodeType.Element)
            {
                return GetXMLTree(node!) + @"/" + args.Name;
            }
            return "";
        }
    }
}
