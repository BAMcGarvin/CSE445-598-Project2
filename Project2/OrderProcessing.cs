/*
 * Description: Our OrderProcessing Class is simply used to verify that the card 
 * number being used by our agencies is valid, and if it is, then it will process 
 * the order using another event-driven method/handler named PO_Processed.
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
    class OrderProcessing
    {

        public static event processPO_Event PO_Processed;
        
        /*
         * Method used to process the ticket agencies order if the cardNo being used is valid.
         */
        public static void orderProcess(OrderClass order, double price)
        {
            // if/else to handle invalid cardNo
            if(!isValid(order.get_cardNo()))
            {
                Console.WriteLine("Please check your card number and re-enter, it is invalid: " + order.get_cardNo());
                return;
            }
            else
            {
                double cost = 0.0;
                cost = (price * order.get_amount());
                // insert processPO event here
                PO_Processed(order.get_senderId(), order.get_cardNo(), order.get_amount(), cost);   // emit our processPO event

            }
        }

        
        /*
         * isValid method to check validity of cardNo being used. A card number is invalid if
         * it is lower that 4000 or higher that 5000
         */
        private static Boolean isValid(int cardNo)
        {
            if(cardNo >= 4000 && cardNo <= 5000)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
                

    }
}
