using ApiShared.Core.Data.BaseInterface;
using ApiShared.Core.Data.Excel.ExcelHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Sample.DbData
{
    [MetadataType(typeof(CHARACTERISTIC_Data))]
    public partial class CHARACTERISTIC : EntityModel
    {

    }

    public class CHARACTERISTIC_Data
    {
        [ExcelColumn(Name = "Code")]
        public int CHARACTERISTIC_CODE { get; set; }

        [ExcelColumn(Name = "Title")]
        public string CHARACTERISTIC_TITLE { get; set; }
    }
}
