using NFeXMLValidator.Helper;
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
                fails.Add(" Alerta: " + NFeHelper.ConvertErrorMessages(args.Message) + " (Caminho: " + NFeHelper.GetPath(args) + ")");
            }
            if (args.Severity == XmlSeverityType.Error)
            {
                fails.Add(" Erro: " + NFeHelper.ConvertErrorMessages(args.Message) + " (Caminho: " + NFeHelper.GetPath(args) + ")");
            }
        }
        
    }
}
