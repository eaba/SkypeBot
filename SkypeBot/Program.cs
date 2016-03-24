using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            List<string> blacklist = new List<string>();






           
            while (true)
            {
                Console.Clear();
                Console.WriteLine("watdo?\n1. Send Message\n2. Clever Responses\n3. Change Mood\n4. Add user to blacklist\n5. Remove user from blacklist\n6. List Contacts");
                int choice;
                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 1:

                            Console.WriteLine("You have selected \"Send Message\"\n");

                            Console.WriteLine("Please enter a contact: ");
                            string contacttosend = Console.ReadLine();
                            Console.WriteLine("Please enter message: ");
                            string messagetosend = Console.ReadLine();
                            skype.SendMessage(contacttosend, messagetosend);
                            break;

                        case 2:

                            Console.WriteLine("You have selected \"Cleverbot Replies\"\n");

                            while (true)
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
                                            Console.WriteLine("Timed out\n" + e);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nUser is on blacklist. ABORT!!1\n");
                                        msg.Seen = true;
                                    }
                                }
                            }
                            break;

                        case 3:
                            Console.WriteLine("You have selected \"Change Mood\"\nPlease enter new status:");
                            string status = Console.ReadLine();
                            skype.CurrentUserProfile.MoodText = status;
                            break;

                        case 4:
                            Console.WriteLine("You have selected \"Add user to blacklist\".\nPlease enter username:");
                            string usertoadd = Console.ReadLine();
                            blacklist.Add(usertoadd);

                            break;
                        case 5:
                            Console.WriteLine("You have selected \"Remove user from blacklist\".\nPlease enter username:");
                            string usertoremove = Console.ReadLine();
                            blacklist.Remove(usertoremove);

                            break;
                        case 6:
                            foreach (User user in skype.Friends)
                            {
                                allcontacts.Add(user.Handle);
                            }
                            Console.WriteLine("\n");
                            allcontacts.ForEach(i => Console.WriteLine("{0}", i));
                            Console.WriteLine("\n");

                            break;
                    }





                    Console.WriteLine("Press Any Key to Continue...");
                    Console.ReadKey();
                }
            }
        }

    }
}
