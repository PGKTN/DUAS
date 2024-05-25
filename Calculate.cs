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

        public double Get_C_fc(double reynolds, double L_avg, double M)
        {
            return 0.455 / (Math.Pow(Math.Log10(Get_Re_L(reynolds, L_avg, M)), 2.58) * Math.Pow((1 + 0.144 * (Math.Pow(M, 2))), 0.65));
        }

        private double Get_Re(double h)
        { 
            return Math.Pow(10, 6) * 
        }
        private double Get_Re_L(double reynolds, double L_avg, double M)
        {
            return reynolds * L_avg * M;
        }
    }
}
