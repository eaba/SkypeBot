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
            foreach (User user in skype.Friends)
            {
                allcontacts.Add(user.Handle);
            }

            allcontacts.ForEach(i => Console.WriteLine("{0}", i));
            Console.WriteLine("\n");



            Console.WriteLine("watdo?\n1. Send Message\n2. Clever Responses\n3. Change Mood");
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

                                Console.WriteLine("Message received from" + msg.Sender.Handle);
                                msg.Seen = true;
                                Console.WriteLine(msg.Body);
                                string reply = "bot> " + bot1session.Think(msg.Body);
                                Console.WriteLine(reply);
                                skype.SendMessage(msg.Sender.Handle, reply);
                                //Console.ReadKey();


                            }
                        }
                        break;

                    case 3:
                        Console.WriteLine("You have selected \"Change Mood\"\nPlease enter new status:");
                        string status = Console.ReadLine();
                        skype.CurrentUserProfile.MoodText = status;



                        break;
                }






                Console.ReadKey();
            }

        }

    }
}
