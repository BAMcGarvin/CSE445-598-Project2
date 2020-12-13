/*
 * Description: Our OrderClass establishes the making of a purchase order. As per project
 * requirements, the recieverID is omitted as I chose to work on this individually. However,
 * each purchase order will contain a senderId to identify what agency is placing the order, a
 * cardNo to make the purchase, the amount of tickets to order, and the price per ticket. The price
 * will be calculated by our Park.
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

namespace Project2
{
    class OrderClass
    {
        // variable declaration
        private string senderId;        // use either thread id or thread name
        private int cardNo;             // represents CC number
        private int amount;             // represents # of tickets to order
        private double price;           // cost of ticket received from park


        public OrderClass()
        {

        /*
         * Ctor with all parameters
         */
        }
        public OrderClass(string senderId, int cardNo, int amount, double price)
        {
            this.senderId = senderId;
            this.cardNo = cardNo;
            this.amount = amount;
            this.price = price;
        }

        /* 
         * Ctor with limited parameters (senderId, cardNo, amount)
         * This constructor will be used 
         */
        public OrderClass(string senderId, int cardNo, int amount)
        {
            this.senderId = senderId;
            this.cardNo = cardNo;
            this.amount = amount;
        }


        // Mutator Methods Below

        /*
         * set_senderId
         */
        public void set_senderId(string ID)
        {
            this.senderId = ID;
        }

        /*
         * set_cardNo
         */
        public void set_cardNo(int Num)
        {
            this.cardNo = Num;
        }

        /*
         * set_amount
         */
        public void set_amount(int amount)
        {
            this.amount = amount;
        }

        /*
         * set_price
         */
        public void set_price(double price)
        {
            this.price = price;
        }


        // Accessor Methods Below

        /*
       * get_senderId
       */
        public string get_senderId()
        {
            return this.senderId;
        }

        /*
         * get_cardNo
         */
        public int get_cardNo()
        {
            return this.cardNo;
        }

        /*
         * get_amount
         */
        public int get_amount()
        {
            return this.amount;
        }

        /*
         * get_price
         */
        public double get_price()
        {
            return this.price;
        }
    }
}
