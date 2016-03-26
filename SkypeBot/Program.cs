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

            Skype skype = new Skype();

            ChatterBotFactory factory = new ChatterBotFactory();
            ChatterBot bot1 = factory.Create(ChatterBotType.CLEVERBOT);
            ChatterBotSession bot1session = bot1.CreateSession();

            //Chat chat = skype.ActiveChats[1];
            //chat.SendMessage("Auto reply " + i);

            List<string> allcontacts = new List<string>();
            List<string> blacklist;
            
            List<string> activesessions = new List<string>();
            int c = 0;
            
            //check if the blacklist file exists. If it does not, it creates it and loads its contents into a list
            try
            {
                blacklist = File.ReadLines("blacklist.txt").ToList();                
            }
            catch
            {
                Console.WriteLine("Blacklist failed to load!\nCreating new one");
                using (StreamWriter sw = File.CreateText("blacklist.txt"))
                    sw.Close();                
            }
            finally
            {
                blacklist = File.ReadLines("blacklist.txt").ToList();
                Console.WriteLine("Blacklist loaded");
                Thread.Sleep(1000);
            }


            while (1 < 2)
            {
                Console.Clear();
                Console.WriteLine("watdo?\n1. Send Message\n2. Clever Responses\n3. Change Mood\n4. Add user to blacklist\n5. Remove user from blacklist\n6. Show blacklist\n7. List Contacts\n\n99. Exit");
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
                            //add user to blacklist
                            Console.WriteLine("You have selected \"Add user to blacklist\".\nPlease enter username:");
                            string usertoadd = Console.ReadLine();
                            blacklist.Add(usertoadd);

                            bool containsuser = File.ReadLines("blacklist.txt").Contains(usertoadd);

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

                        case 5:
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

                        case 6:
                            //display blacklist to user
                            Console.WriteLine("\n");
                            blacklist.ForEach(i => Console.WriteLine("{0}", i));
                            Console.WriteLine("\n");
                            break;

                        case 7:
                            //display all contacts to user
                            foreach (User user in skype.Friends)
                            {
                                allcontacts.Add(user.Handle);
                            }
                            Console.WriteLine("\n");
                            allcontacts.ForEach(i => Console.WriteLine("{0}", i));
                            Console.WriteLine("\n");
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
