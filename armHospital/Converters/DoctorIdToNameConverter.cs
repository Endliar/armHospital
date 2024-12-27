using armHospital.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace armHospital.Converters
{
    public class DoctorIdToNameConverter
    {
        private static Dictionary<int, string> _doctorNamesCache = new Dictionary<int, string>();

    }
}
