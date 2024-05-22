using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DUAS
{
    public class Calculate
    {
        public double Get_P_i(double T_rotor, double vi)
        {
            return T_rotor * vi;
        }

        public double Get_P_0(double sigma, double Cd0, double rho, double A_disk, double v_tip)
        { 
            return ((sigma * Cd0) / 8) * rho * A_disk * Math.Pow(v_tip, 3);
        }
    }
}
