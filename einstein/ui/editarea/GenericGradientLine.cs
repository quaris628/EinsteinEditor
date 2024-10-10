using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public interface GenericGradientLine
    {
        Color StartColor { get; set; }
        Color EndColor { get; set; }
    }
}
