/*
 * Description: This is our main program. It is specifically used
 * to simulate everything. It will create the buffer classes & delagates,
 * instantiate the objects, create threads and start threads.
 * 
 * Project 2 (Assignments 3 & 4)
 * CSE 445/598 Distributed Software Development
 * Session C Fall 2020
 * Dr. Yinong Chen
 * 
 * Author:Bradley McGarvin
 * 
 * References: 7th edition Service-Oriented Computing and System Integration,
 * 2.6.3 Case Study, docs.microsof.com in refernce to semphores, locks, and monitors
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project2
{    
    
    public delegate void newPO_Event();                                                             // define a newPo Event
    public delegate void priceCutEvent(string senderId, double price);                              // define a piceCut Event
    public delegate void priceChangeEvent(string senderId, double price);                           // define a priceChange Event
    public delegate void processPO_Event(string senderId, int cardNo, int amount, double price);    // define a processPO Event

    class Program
    {

        public static Thread[] ticketAgencies;          // global variable for our ticketAgency threads
        public static MultiCellBuffer MCB;              // global variable for our MultiCellBuffer
        public static bool parkThreadRunning = true;    // boolean variable to keep track of whether or not our parkThread is running


        /*
         *  Main program to simulate our Ticket Agencies making orders from our Park
         */
        static void Main(string[] args)
        {

            Park park = new Park();                                 // instantiate a Park object named park
            TicketAgencies ticketAgency = new TicketAgencies();     // instantiate a TicketAgencies object named ticketAgency

            MCB = new MultiCellBuffer(3);                           // instantiate our MCB with 3 cells per assignment req.

            Thread parkThread = new Thread(new ThreadStart(park.parkCtor)); // create our parkThread with our park Ctor


            parkThread.Start();                                     // start the park Thread

            Park.priceCut += new priceCutEvent(ticketAgency.ticketSale);    // use event handler when a priceCut (ticketSale) occurs

            Park.priceChange += new priceChangeEvent(ticketAgency.ticketIncrease);  // use event handler when a price increase occurs
            
            TicketAgencies.newPO += new newPO_Event(park.processPO);        // process our order once a newPO event is emitted
            
            OrderProcessing.PO_Processed += new processPO_Event(ticketAgency.PO_Processed); // callback for when order is processed
            
            ticketAgencies = new Thread[5]; // [per project requirements . . . for loop to create 5 ticketAgency threads]

            // in our for loop we create a new thread using our ticketAgency ctor and we set the name (senderId) 
            // for each ticket agency with numbers 1 - 5. We then start each thread.
            for(int i = 0; i < 5; i++)
            {
                ticketAgencies[i] = new Thread(new ThreadStart(ticketAgency.ticketAgency));
                ticketAgencies[i].Name = (i + 1).ToString();

                ticketAgencies[i].Start();
                
            }
           


        }
    }
}
