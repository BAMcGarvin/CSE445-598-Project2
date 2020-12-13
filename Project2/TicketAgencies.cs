/*
 * Description: Our TicketAgencies class is responsible for constructing our ticket agencies.
 * Our main program will instantiate each ticket agency as a thread and the agencies will be
 * event-driven such that each agency will include callback methods (event handlers) to indicate
 * price changes from our park. The Agencies will calculate the number of tickets to order based on
 * a random need, however, if the price change goes up, they will often order less tickets than if 
 * the price were cheaper. The agencies will send their purhase orders to the MCB to be processed by
 * the park.
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
    class TicketAgencies
    {
        // Variable Declaration
        Random rand = new Random();             // used to generate random numbers

        public static event newPO_Event newPO;              // an event to let our Park know that there is a new purchase order

        /*
         * The Ticket Agency method is used to help determine when to end the thread.
         * The thread for each new agency will end once the park thread that called it ends.
         * This is used to create ticket purchase orders.
         */
        public void ticketAgency()
        {

            // while the parkThread is running, so too will the ticketAgency thread.
            while (Program.parkThreadRunning)
            {
                Thread.Sleep(2000);
                ticketPO(Thread.CurrentThread.Name, 199.99);
            }
        }

        /*
         * The ticket purchase order (PO) method is used to create a purchase order 
         * and to send it to our multicellbuffer (MCB).
         */
        private void ticketPO(string threadID, double price)
        {
            int cardNo = rand.Next(4000, 5000);     // generate a random number to signify our CC #.
            int amount = rand.Next(30, 100);        // generate a random number to signify # of tickets needed.

            // instantiate and OrderClass object to hold our ticket purchase order
            OrderClass purchaseOrder = new OrderClass(threadID, cardNo, amount, price);

            // implementation of timestamp before sending purchase order to MCB.
            Console.WriteLine("Ticket Agency {0} has a new purchase order for {1} tickets at {2}.", threadID, amount, DateTime.Now.ToString("hh:mm:ss"));

            
            Program.MCB.setOneCell(purchaseOrder);      // place purchase order into MCB
            
            newPO();                                    // emits event

            

        }

        /*
         * The higherTicketPO method is used to create a purchase order based on the pricing event.
         * This specifically handles when the price increase, so that the ticket agency demand for 
         * amount of tickets will go down to ensure they order more tickets when the price is cheaper.
         */
        private void higherTicketPO(string threadID, double price)
        {
            int cardNo = rand.Next(4000, 5000);     // generate a random number to signify our CC #.
            int amount = rand.Next(10, 40);         // generate a lower need for tickets when price is higher

            // instantiate and OrderClass object to hold our ticket purchase order
            OrderClass purchaseOrder = new OrderClass(threadID, cardNo, amount, price);

            // implementation of timestamp before sending purchase order to MCB.
            Console.WriteLine("Ticket Agency {0} has a new purchase order for {1} tickets at {2}.", threadID, amount, DateTime.Now.ToString("hh:mm:ss"));


            Program.MCB.setOneCell(purchaseOrder);      // place purchase order into MCB

            newPO();                                    // emits event

        }


        /*
         * EVENT HANDLER (for priceCut event)
         */
        public void ticketSale(string senderId, double price)
        {
            // create a purchase order for tickets from our park and place in queue.    
            Console.WriteLine("ATTENTION: New Ticket Sale for Ticket Agencies: Everyday low price of ${0}.", price);
            ticketPO(senderId, price);
        }


        /*
         * EVENT HANDLER (for priceChange event)
         */
        public void ticketIncrease(string senderId, double price)
        {
            // create a purchase order for tickets from our park and place in queue
            Console.WriteLine("The Park has ended their sale on tickets, the new price is ${0}.\nMore sales coming soon!\n", price);
            higherTicketPO(senderId, price);
        }


        /*
         * EVENT HANDLER (signifies when the purchase order has been processed. Write to console.)
         */
        public void PO_Processed(string senderId, int cardNo, int amount, double price)
        {
            double costPerTicket = price / amount;

            Console.WriteLine("Ticket Agency {0}: Thank you for your order of " + amount + 
                " tickets. At $" + costPerTicket + "/ticket, the \ntotal price paid is $" + price + 
                ". We have charged your credit card ending in: {1}\n", senderId, cardNo);

 /*           if (Program.parkThreadRunning == false)
            {
                Console.WriteLine("The last purchase orders for each Ticket Agency have been processed. \nHave a good day and thank you for your purchase.");
                Console.WriteLine("We have had 20 priceCut events. All threads are now terminating. This concludes our simulation");
            }
 */
        }
    }
}
