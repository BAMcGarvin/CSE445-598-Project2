/*
 * Description: Our Park class is used to help create a park,
 * manage the price changes and price events along with incoming orders
 * from ticket agencies. It will also help to process these purchase orders.
 * We use our park constructor as a starting point for our thread in the main
 * program.
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
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Project2
{
    class Park
    {

        public static event priceCutEvent priceCut;         // link to delegate 
        public static event priceChangeEvent priceChange;   // an event to handle when the price change is not cheaper
        static Random rand = new Random();          // this will be used to help generate random numbers

        private static double ticketPrice = 199.99; // initial ticket price for all agencies 
        private static int index = 0;               // index to keep track agency threads and prevent exception error
        private static int count = 0;               // keep track of priceCut events
        

        /*
         * Constructor that will be used to create our park threads in our main program. We will 
         * instantiate a Park object named park and use it to call park.parkCtor in our parkThread.
         * After 20 price cuts have been made the parkThread will terminate. A priceCut is defined 
         * only when the price decreases.
         */
        public void parkCtor()
        {
            // while loop to keep our parkThread running until 20 (P) priceCut events have occurred
            while (getCount() < 20)
            {
                Thread.Sleep(500);              // generates a new price every 1/2 second
                double price = PricingModel();  // set our price by calling our pricing model
                changePrice(price);             // account four our price change whether it goes up or down.
                
            }

            Program.parkThreadRunning = false; // lets agency threads know that the park thread has ended
            
        }

        /*
         * Mutator method to set our count for priceCut events
         */
        public void setCount(int i)
        {
            count = i;
        }

        /*
         * Accessor Method to access our count of priceCut events
         */
        public int getCount()
        {
            return count;
        }

        /*
         * Accessor method to access our ticketPrice
         */
        public double get_ticketPrice()
        {
            return ticketPrice;
        }


        /*
         * Method: changePrice
         * Allows for the change in price regardless of whether our new price in higher 
         * or lower than the previous price. If prices is higher, we call a different 
         * pricing event called changePrice. We only increment our count when an actual 
         * priceCut occurs.
         */
        public void changePrice(double price)
        {

            int i = getCount();

            // exception handler (index out of range exception)
            // if the index iterates all the way up to 5, we reset the index back to 0
            if (Program.ticketAgencies.Length == index)
                index = 0;

            // Functionally equivalent to Monitor.Enter/Exit
            // help to manage read/write permissions
            lock (this)
            {
                // if there is at least one subscriber
                if (priceCut != null && priceChange != null)
                {
                    // if statement to ensure priceCut event is emitted only if new prices is lower.
                    // increment index and count.
                    if (price < ticketPrice)
                    {
                        
                        priceCut(Program.ticketAgencies[index].Name, price);    // call our priceCut eventhandler

                        ticketPrice = price;
                        index++;
                        i++;
                        setCount(i);

                    }

                    // if statement to handle all price changes that are higher than existing ticketPrice
                    if (price > ticketPrice)
                    {
               
                        priceChange(Program.ticketAgencies[index].Name, price); // call our priceChange eventhandler
                        ticketPrice = price;
                        index++;
                    }
                }
            }
        }

        /*
         * Method: PricingModel
         * This method generates a random price for tickets between the price of $80 - $300
         * returns the price:double
         */
        public double PricingModel()
        {
            //  decides the price of tickets, which must
            // be between 80 and 300.It can increase or decrease the price. You must define a 
            // mathematical model (formula). The model can be a simple random function for individual projects.
            
            double price = rand.Next(80, 300); //generates a random price between $80 and $300

/* Implemented change price method instead
 
            // exception handler 
            if (Program.ticketAgencies.Length == index)
            {
                index = 0;
            }

            // if there is at least one subscriber
            if (priceCut != null)
            {
                // if statement to ensure priceCut event is emitted only if new prices is lower.
                // increment index and count.
                if (price < ticketPrice)
                {
                    
                    priceCut(Program.ticketAgencies[index].Name, price);
                    index++;
                    count++;
                }

                // if statement to handle all price changes that are higher than existing ticketPrice
                if (price > ticketPrice)
                {
                    
                    ticketPrice = price;
                   // double temp = ticketPrice;
                   // price = ticketPrice;
                    index++;
                }
            }
 */           
            return price;
        }


        /*
         * EVENT HANDLER (this will be used to actually process our purchase order.)
         * It receives the orders from the MultiCellBuffer (MCB). For each order, a new thread is started
         * (resulting in multiple threads for processing multiple orders) from OrderProcessing class in
         * order to process the purchase order based on the current price.
         */
        public void processPO()
        {

            OrderClass purchaseOrder = Program.MCB.getOneCell();    // retrieves our PO from our MCB

            // creates a new thread for each new order from the OrderProcessing class in order to process our purchase order.
            Thread processPO_Thread = new Thread(() => OrderProcessing.orderProcess(purchaseOrder, get_ticketPrice()));


            processPO_Thread.Start();



        }

    }
}
