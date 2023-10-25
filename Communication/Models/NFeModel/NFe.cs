using Communication.Models.NFeModel.Info;
using System.Xml.Serialization;

namespace Communication.Models.NFeModel
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class NFe
    {
        /// <summary>
        ///     A01 - Informações da Nota Fiscal Eletrônica
        /// </summary>
        [XmlElement(Namespace = "http://www.portalfiscal.inf.br/nfe")]
        public infNFe infNFe { get; set; }
    }
}
