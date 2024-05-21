using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//////////////////////////////////////////////////////////
/// P -> Power, W -> Weight, E -> Energy, V -> Velocity///
//////////////////////////////////////////////////////////


namespace DUAS
{
    internal class main
    {
        static void Main(string[] args)
        {
            //////////////
            /// Weight ///
            //////////////
            double W_gross = 0.0;
            double W_empty = 0.0;
            double Payload = 0.0;



            ////////////////////////////////
            /// Propulsion System Sizing ///
            ////////////////////////////////
            double P_req = 0.0;
            double P_max = 0.0;
            double P_takeoff = 0.0;
            double P_climb1 = 0.0;
            double P_climb2 = 0.0;
            double P_cruise = 0.0;
            double P_decent = 0.0;

            List<double> Powers = new List<double>();
            Powers.Add(P_takeoff);
            Powers.Add(P_climb1);
            Powers.Add(P_climb2);
            Powers.Add(P_cruise);
            Powers.Add(P_decent);

            P_max = Powers.Max();

            List<double> P_analysis = new List<double>();
            double V_max = 0.0;

            List<double> P_perform = new List<double>();
            for (int i = 0; i < V_max; i++)
            {
                P_perform.Add(P_analysis[i]);
            }
            

            ////////////////////////
            /// Mission Analysis ///
            ////////////////////////
            double Energy_takeoff = 0.0;
            double Energy_cruise = 0.0;
            double Energy_landing = 0.0;

            List<double> Energies = new List<double>();
            Energies.Add(Energy_takeoff);
            Energies.Add(Energy_cruise);
            Energies.Add(Energy_landing);

            double Energy_total = Energies.Sum();



        }
    }
}