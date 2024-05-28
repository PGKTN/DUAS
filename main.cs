using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


///////////////////////////////////////////////////////////
/// P -> Power, W -> Weight, E -> Energy, V -> Velocity ///
/// 기본 단위 : m, kg, s                                 ///
///////////////////////////////////////////////////////////


namespace DUAS
{
    internal class main
    {

        static void Main(string[] args)
        {
            Calculate Calc = new Calculate();
            ReadWriteINIfile ini = new ReadWriteINIfile(@"C:\DUAS\rotor.ini");

            double rho_0 = 1.225;
            double g = 9.81;

            double W_empty = 1600;
            double W_payload = 500;
            double W_0 = W_empty + W_payload;

            double N_rotor = 4;
            double rotor_diameter = 3;
            double sigma = 0.177;
            double A_disk = Math.PI * Math.Pow(rotor_diameter / 2, 2);
            double rpm = 2000;
            double omega = rpm * (2 * Math.PI) / 60
;
            double root_mainwing = 1.8;
            double tip_mainwing = 1.2;
            double span_mainwing = 7;
            double sweep_mainwing = 3.5; //[˚]
            double A_mainwing = (root_mainwing + tip_mainwing) * span_mainwing;
            double chord_mac_mainwing = (A_mainwing / 2) / span_mainwing;

            double mac_mainwing = chord_mac_mainwing / 4;

            double root_htail = 1;
            double tip_htail = 1;
            double span_htail = 2.5;
            double A_htail = (root_htail + tip_htail) * span_htail;
            double mac_htail = ((A_htail / 2) / span_htail) / 4;

            double Cd0 = 0.03360;
            double S_ref = 97.931721; // [m²]

            double Xloc_mainwing = 1.721;
            double Xloc_htail = 7.5;



            


            ///////////////
            /// P_hover ///
            ///////////////
            double T_hover = W_0 * g;
            double T_rotor_hover = T_hover / N_rotor;

            double t_hover = 0.0;
            double h_hover = 300;
            double rho_hover = Calc.Get_rho(rho_0, h_hover);

            double vi_hover = Math.Sqrt(T_rotor_hover / (2 * rho_hover * A_disk));

            double v_tip = omega * (rotor_diameter / 2);

            double P_i_hover = Calc.Get_P_i(T_rotor_hover, vi_hover);
            double P_0_hover = Calc.Get_P_0(sigma, Cd0, rho_hover, A_disk, v_tip);

            double P_req_hover = P_i_hover + P_0_hover;

            double E_hover = P_req_hover * (t_hover / 3600);

            ///////////////
            /// P_climb ///
            ///////////////
            double T_climb = W_0 * g;
            double T_rotor_climb = T_climb / N_rotor;

            double t_climb = 3 * 60;
            double h_climb = 0.0;
            double rho_climb = Calc.Get_rho(rho_0, h_climb);

            double v_climb = 3.333333333;
            double vi_climb = (-v_climb / 2) + Math.Sqrt(Math.Pow(v_climb / 2, 2) + (T_rotor_climb / (2 * rho_climb * A_disk)));

            double P_i_climb = Calc.Get_P_i(T_rotor_climb, vi_climb);
            double P_0_climb = Calc.Get_P_0(sigma, Cd0, rho_climb, A_disk, v_tip);

            double P_req_climb = P_i_climb + P_0_climb;

            double E_climb = P_req_climb * (t_climb / 3600);


            /////////////////
            /// P_forward ///
            /////////////////
            double t_forward = 18 * 60;
            double h_forward = 600;
            double rho_forward = Calc.Get_rho(rho_0, h_forward);

            double v_forward = 55.55555556;

            double D_forward = 0.5 * rho_forward * Math.Pow(v_forward, 2) * Cd0 * S_ref;

            double P_req_forward = D_forward * v_forward;

            double E_forward = P_req_forward * (t_forward / 3600);

            double Cl_mainwing = 0.52;
            double Cd_mainwing = 0.00855;

            double L_mainwing_forward = 0.5 * rho_forward * Math.Pow(v_forward, 2) * A_mainwing * Cl_mainwing;
            double D_mainwing_forward = 0.5 * rho_forward * Math.Pow(v_forward, 2) * A_mainwing * Cd_mainwing;




            /////////////////
            /// P_descent ///
            /////////////////
            double T_descent = W_0 * g;
            double T_rotor_descent = T_descent / N_rotor;

            double t_descent = 9 * 60;
            double h_descent = 600;
            double rho_descent = Calc.Get_rho(rho_0, h_descent);

            double v_descent = -1.111111111;
            double vi_descent = (-v_descent / 2) + Math.Sqrt(Math.Pow(v_descent / 2, 2) + (T_rotor_descent / (2 * rho_descent * A_disk)));

            double P_i_descent = Calc.Get_P_i(T_rotor_descent, vi_descent);
            double P_0_descent = Calc.Get_P_0(sigma, Cd0, rho_descent, A_disk, v_tip);

            double P_req_descent = P_i_descent + P_0_descent;

            double E_descent = P_req_descent * (t_descent / 3600);

            double E_total = E_hover + E_climb + E_forward + E_descent;



            Console.WriteLine("T_hover : " + Math.Round(T_hover) + " [N]");

            Console.WriteLine("P_req_hover : " + Math.Round(P_req_hover) / 1000 + " [kW]");
            Console.WriteLine("P_req_climb : " + Math.Round(P_req_climb) / 1000 + " [kW]");
            Console.WriteLine("P_req_forward : " + Math.Round(P_req_forward) / 1000 + " [kW]");
            Console.WriteLine("P_req_descent : " + Math.Round(P_req_descent) / 1000 + " [kW]");

            Console.WriteLine("E_hover : " + Math.Round(E_hover) / 1000 + " [kWh]");
            Console.WriteLine("E_climb : " + Math.Round(E_climb) / 1000 + " [kWh]");
            Console.WriteLine("E_forward : " + Math.Round(E_forward) / 1000 + " [kWh]");
            Console.WriteLine("E_descent : " + Math.Round(E_descent) / 1000 + " [kWh]");

            Console.WriteLine("E_total : " + Math.Round(E_total) / 1000 + " [kWh]");

            Console.WriteLine("L_mainwing_forward : " + Math.Round(L_mainwing_forward) + "");
            Console.WriteLine("D_mainwing_forward : " + Math.Round(D_mainwing_forward) + "");



            double L_htail = (Xloc_htail - Xloc_mainwing) - mac_mainwing + mac_htail;
            double C_ht = 0.0;

            while (Math.Abs(C_ht - 0.535) > 10e-5)
            {
                C_ht = (A_htail * L_htail) / (A_mainwing * chord_mac_mainwing);
                root_htail = root_htail * 0.999;
                tip_htail = root_htail;

                A_htail = (root_htail + tip_htail) * span_htail;
            }

            Console.WriteLine(root_htail) ;
            Console.WriteLine(A_htail);
            Console.WriteLine(C_ht);
        }
    }
}