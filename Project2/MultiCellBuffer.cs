/*
 * Description: Our MultiCellBuffer class is used for the communication between 
 * the ticket agencies(clients) and the park(server). In this class, I implemented 
 * 3 data cells instead of 2. These cells will hold our purchase order details. The
 * MCB is solely responsible for managing cell availability along with read/write
 * permissions.
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
using System.Threading;
using System.Threading.Tasks;

namespace Project2
{
    class MultiCellBuffer
    {

        private static Semaphore get_pool;      // creates a "pool" for reading
        private static Semaphore set_pool;      // creates a "pool" for writing
        private static Semaphore rw_pool;       // read/write "pool"
 //       public OrderClass[] purchaseOrders;
        private int n;                          // stores the number of buffer cells
        private int nCount;                     // to keep track of occupied buffer cells
        List<OrderClass> purchaseOrders;        // instantiate our purchaseOrders List for our MCB


        /*
         *  Our MultiCellBuffer helps to instantiate our MCB
         *  Parameter is of type int and sets the total size of cells that will be available at one time
         *  create semaphores for getting (reading) and setting (writing) 
         */
        public MultiCellBuffer(int n)
        {          
            this.n = n;

            get_pool = new Semaphore(n, n);        // sets our semaphore for reading to 3 to show availabe reading resources
            set_pool = new Semaphore(n, n);       // sets our semaphore for writing to 3 to show available writing resources
 //           rw_pool = new Semaphore(n, n);

 //           purchaseOrders = Enumerable.Range(1, n).Select(i => new OrderClass()).ToList(); //Limits the number of iterations to n

            purchaseOrders = new List<OrderClass>(n);   // initialize size of list

        }


        /*
         * Our setOneCell method is used to write data to our cells
         * A semaphore is used to manage the availability of cells and a lock is placed on
         * our list of OrderClass objects (purchase orders) to manage read/write permissions
         * and we also implement additional monitors to prevent writing to a full MCB.
         */
        public void setOneCell(OrderClass PO)
        {
            set_pool.WaitOne();   // requesting a write resource/enter the set_pool semaphore
//            rw_pool.WaitOne();

            // functionally equivalent to Monitor.Enter/Monitor.Exit
            lock (purchaseOrders)
            {
                
                if(nCount == n)
                {
                    // block other threads from trying to send a new PO to MCB while PO count = 3
                    Monitor.Wait(purchaseOrders);  
                }   
 
                // for loop to insert our purchase order into the next available cell in our list
                for(int i = 0; i < n; i++)
                {
                    // need to check to see if a PO already exists
                    //if (purchaseOrders.ElementAt(i).Equals(PO))                  
                    //if(purchaseOrders.ElementAt(i) == null)
                    {
                        purchaseOrders.Insert(i, PO);
                        nCount++;   // increment our nCount
                        i = n;      // exiting loop once PO is added to avoid duplicates
                        
                    }
                }
/*
                for(int i = 0; i < n; i++)
                {
                    
                    if(purchaseOrders[i] == null)
                    {
                        
                        purchaseOrders[i] = PO;
                        nCount++;                   // increment cell count to show one less cell is available 
                        i = n;                      // set i = n to exit loop and prevent duplicate POs
                        
                    }
                }
*/
                set_pool.Release();   // release the semaphore once
 //               rw_pool.Release();
                Monitor.Pulse(purchaseOrders);    // send notification to our thread that cell is available
               
            }
        }


        /*
         * Our getOneCell method is used to read data (purchase orders) from our cells
         * A semaphore is used to block our thread until a signal is received to read data. 
         * We lock our list of OrderClass objects (purchase orders) to manage read/write permissions
         * and we also implement additional monitors to prevent trying to read from an empty MCB.
         */
        public OrderClass getOneCell()
        {
            OrderClass PO = null;

            get_pool.WaitOne();
//            rw_pool.WaitOne();

            // functionally equivalent to Monitor.Enter/Monitor.Exit
            lock (purchaseOrders)
            {
                // once our cells are empty, block threads from trying to get POs that do not exist
                if(nCount == 0)
                {
                    Monitor.Wait(purchaseOrders);
                }

                // for loop to read our purchase order from each cell in our list
                for (int i= 0; i < n; i++)
                {
                    if(purchaseOrders[i] != null)   // makes sure there is something to read a each cell
                    {
                        PO = purchaseOrders[i];     // Copy our PO details
                        purchaseOrders.RemoveAt(i); // Remove PO from MCB
                        nCount--;                   // Decrement our count
                        i = n;                      // terminate loop to ensure we only get 1 cell
                    }
                }

/*
                for(int i = 0; i < n; i++)
                {
                    if(purchaseOrders[i] != null)
                    {
                        PO = purchaseOrders[i];
                        purchaseOrders[i] = null;   // reset this cell to null
                        nCount--;                   // decrement the to signify a cell is available for a new ticket PO
                        i = n;                      // exit the loop in order to prevent setting all cells to null
                    }
                }

 */
                get_pool.Release();
 //               rw_pool.Release();
                Monitor.Pulse(purchaseOrders);
            }

            return PO;
        }
    }
}
