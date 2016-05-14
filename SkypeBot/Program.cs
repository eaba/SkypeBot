using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using SKYPE4COMLib;
using ChatterBotAPI;

namespace SkypeBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started!!");
            Console.Title = "Skype Toolkit";

            Skype skype = new Skype();
            skype.Attach(7, false);

            //Chat chat = skype.ActiveChats[1];
            //chat.SendMessage("Auto reply " + i);

            List<string> allcontacts = new List<string>();
            List<string> blacklist;

            List<string> activesessions = new List<string>();

            bool containsuser;

            //add all contacts to a list
            foreach (User user in skype.Friends)
            {
                allcontacts.Add(user.Handle);
            }

            //check if the blacklist file exists. If it does not, it creates it and loads its contents into a list
            try
            {
                blacklist = File.ReadLines("true.dat").ToList();
            }
            catch
            {
                Console.WriteLine("Blacklist failed to load!\nCreating new one");
                using (StreamWriter sw = File.CreateText("true.dat"))
                    sw.Close();
            }
            finally
            {
                blacklist = File.ReadLines("true.dat").ToList();
                Console.WriteLine("Blacklist loaded");
                Thread.Sleep(1000);
            }

            while (1 < 2)
            {
                Console.Clear();
                Console.WriteLine("watdo?\n1. Send Message\n2. Clever Responses\n3. Change Mood\n4. Spam status\n5. Add user to blacklist\n6. Remove user from blacklist\n7. Show blacklist\n8. List Contacts\n9. Interpret commands\n\n99. Exit");
                int choice;
                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 1:
                            //send message
                            Console.WriteLine("You have selected \"Send Message\"\nPlease enter a contact: ");
                            string contacttosend = Console.ReadLine();
                            Console.WriteLine("Please enter message: ");
                            string messagetosend = Console.ReadLine();
                            skype.SendMessage(contacttosend, messagetosend);
                            break;
                            
                        case 2:
                            //respond to unread messages with cleverbot's replies
                            Console.WriteLine("You have selected \"Cleverbot Replies\"\n\nPress any key to stop.");

                            ChatterBotFactory factory = new ChatterBotFactory();
                            ChatterBot bot1 = factory.Create(ChatterBotType.CLEVERBOT);
                            ChatterBotSession bot1session = bot1.CreateSession();

                            while (!Console.KeyAvailable)
                            {
                                foreach (IChatMessage msg in skype.MissedMessages)
                                {
                                    //string handle = msg.Sender.Handle;
                                    //string message = "test";
                                    //skype.SendMessage(handle, message);

                                    if (!blacklist.Contains(msg.Sender.Handle))
                                    {
                                        try
                                        {
                                            Console.WriteLine("Message received from [" + msg.Sender.Handle + "]\n");
                                            msg.Seen = true;
                                            Console.WriteLine("Message: [" + msg.Body + "]\n");
                                            string reply = "bot> " + bot1session.Think(msg.Body);
                                            Console.WriteLine("Reply: [" + reply + "]\n");
                                            skype.SendMessage(msg.Sender.Handle, reply);
                                        }
                                        catch (Exception e)
                                        {
                                            //usually a timeout
                                            Console.WriteLine("Timed out..\n" + e);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nUser is on blacklist. ABORT!!1\n");     //ext0
                                        msg.Seen = true;
                                    }
                                }
                            }

                            break;

                        case 3:
                            //change skype mood
                            Console.WriteLine("You have selected \"Change Mood\"\nPlease enter new status:");
                            string status = Console.ReadLine();
                            skype.CurrentUserProfile.MoodText = status;
                            break;

                        case 4:
                            //spam skype status
                            Console.WriteLine("You have selected \"Spam Status\"\nPress any key to stop.");
                            int c=0;
                            while (!Console.KeyAvailable)
                            {
                                if (c == 0) skype.ChangeUserStatus(TUserStatus.cusOnline);
                                else if (c == 1) skype.ChangeUserStatus(TUserStatus.cusAway);
                                else if (c == 2) skype.ChangeUserStatus(TUserStatus.cusDoNotDisturb);
                                else if (c == 3)
                                {
                                    skype.ChangeUserStatus(TUserStatus.cusInvisible);
                                    c = 0;
                                }
                                c++;    //hehehehehe
                            }
                            break;

                        case 5:
                            //add user to blacklist
                            Console.WriteLine("You have selected \"Add user to blacklist\".\nPlease enter username:");
                            string usertoadd = Console.ReadLine();
                            blacklist.Add(usertoadd);

                            containsuser = File.ReadLines("blacklist.txt").Contains(usertoadd);

                            if (!containsuser == true)
                            {
                                TextWriter tw = new StreamWriter("blacklist.txt");
                                blacklist.ForEach(tw.WriteLine);
                                tw.Close();
                                Console.WriteLine("Added user to blacklist.");
                            }
                            else
                            {
                                Console.WriteLine("User is already blacklisted.");
                            }

                            break;

                        case 6:
                            //remove user from blacklist
                            Console.WriteLine("You have selected \"Remove user from blacklist\".\nPlease enter username:");
                            string usertoremove = Console.ReadLine();

                            containsuser = File.ReadLines("blacklist.txt").Contains(usertoremove);
                            if (containsuser == true)
                            {
                                blacklist.Remove(usertoremove);
                                File.WriteAllLines(("blacklist.txt"), blacklist.ToList());
                                Console.WriteLine("User removed from blacklist.");
                            }
                            else
                            {
                                Console.WriteLine("User is not on blacklist.");
                            }
                            break;

                        case 7:
                            //display blacklist to user
                            Console.WriteLine("\n");
                            blacklist.ForEach(i => Console.WriteLine("{0}", i));
                            Console.WriteLine("\n");
                            break;

                        case 8:
                            //display all contacts to user
                            Console.WriteLine("\n");
                            allcontacts.ForEach(i => Console.WriteLine("{0}", i));
                            Console.WriteLine("\n");
                            break;

                        case 9:
                            //commands
                            while (!Console.KeyAvailable)
                            {
                                foreach (IChatMessage msg in skype.MissedMessages)
                                {
                                    msg.Seen = true;
                                    string trigger = "!";

                                    if (!blacklist.Contains(msg.Sender.Handle) && msg.Body.IndexOf(trigger) == 0)
                                    {
                                        string command = msg.Body.Remove(0, trigger.Length).ToLower();
                                        string message;

                                        if (command == "time")
                                        {
                                            message = DateTime.Now.ToLongTimeString();
                                        }
                                        else if (command == "date")
                                        {
                                            message = DateTime.Now.ToLongDateString();
                                        }
                                        else if (command == "about")
                                        {
                                            message = "made by root 2016 and such and such. Licensed under the 'DO WHAT THE FUCK YOU WANT TO' public license. https://rootme.tk";
                                        }
                                        else if (command == "help")
                                        {
                                            message = "Help: Commands include: !time !date !about !int2binary !binary2int !blacklist !help !collatzcon";
                                        }
                                        else if (command == "blacklist")
                                        {
                                            usertoadd = msg.Sender.Handle;
                                            containsuser = File.ReadLines("blacklist.txt").Contains(usertoadd);
                                            if (!containsuser == true)
                                            {
                                                blacklist.Add(msg.Sender.Handle);
                                                message = "Added to blacklist.";
                                                TextWriter tw = new StreamWriter("blacklist.txt");
                                                blacklist.ForEach(tw.WriteLine);
                                                tw.Close();
                                                Console.WriteLine("Added user to blacklist.");
                                            }
                                            else
                                            {
                                                message = "This should never be seen by user..";    
                                            }
                                        }
                                        else                  //magic dont touch. seriously.
                                        {
                                            try           //not the most elegant solution
                                            {
                                                if (command.Substring(0, 10) == "int2binary")     //this usually breaks so dont scare it   
                                                {
                                                    string inttoconvert = command.Substring(10, command.Length - 10);                                                    
                                                    string binary = Convert.ToString(Convert.ToInt32(inttoconvert), 2);

                                                    message = inttoconvert + " in binary is: " + binary;
                                                }
                                                else if (command.Substring(0, 10) == "binary2int") //dont scare this guy too
                                                {
                                                    string bits = command.Substring(11, command.Length - 11);                                                    
                                                    int convertedbinary = Convert.ToInt32(bits, 2);
                                                    message =  bits + " in decimal is: " + convertedbinary.ToString();
                                                }
                                                else if (command.Substring(0,10) == "collatzcon") 
                                                {
                                                    int number = int.Parse(command.Substring(11, command.Length - 11));
                                                    c = 0;
                                                    string premessage = number.ToString();
                                                    do {
                                                    
                                                        if (number % 2 == 0) //even
                                                        {
                                                            number = number / 2;
                                                            //Console.WriteLine("even");
                                                        }
                                                        else //odd
                                                        {
                                                            number = number * 3 + 1;
                                                            //Console.WriteLine("odd");
                                                        }
                                                        c++; //hehehe i gotta stop
                                                        premessage += "," + number;
                                                        
                                                    } while (number != 1);

                                                    //dont really like this
                                                    skype.SendMessage(msg.Sender.Handle, premessage);
                                                    message = "That took " + c + "iterations.";
                                                 
                                                }
                                                else
                                                {
                                                    message = "Unknown Command";
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                message = "Unknown Command";                                                
                                            }
                                            
                                        }


                                        Console.WriteLine(msg.Sender.Handle + " >> " + command);
                                        skype.SendMessage(msg.Sender.Handle, message);


                                        //add: binary/hex/decimal converter
                                    }
                                }
                            }
                            break;
                        case 99:
                            Environment.Exit(0);
                            break;
                    }
                    Console.WriteLine("Press Any Key to Continue...");
                    Console.ReadKey();
                }
            }
        }
    }
}
