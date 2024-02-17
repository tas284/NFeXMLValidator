using System.Xml;
using System.Xml.Schema;

namespace NFeXMLValidator.Helper
{
    public static class NFeHelper
    {
        public static string ConvertErrorMessages(string message)
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
        public static string GetPath(ValidationEventArgs args)
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
