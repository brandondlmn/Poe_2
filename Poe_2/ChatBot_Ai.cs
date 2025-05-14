using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System;
using System.Drawing;

namespace Poe_2
{
    public class ChatBot_Ai
    {
        // Declaring and initializing variables
        private List<string> replies = new List<string>();
        private List<string> ignore = new List<string>();
        private MemoryManager memoryManager = new MemoryManager();
        private readonly Random _random = new Random();
        private string userName;
        private string userInterest; // Store user's cybersecurity interest
        private List<string> userInterests = new List<string>();
        private List<string> conversationHistory = new List<string>();

        // Delegate declarations
        private delegate string ResponseHandler(string sentiment, bool isFollowUp, List<string> usedResponses);
        private delegate bool QuestionMatcher(string question);

        // Constructor
        public ChatBot_Ai()
        {
            memoryManager.check_file();
            Logo();
            Greeting();
            userInfo();
            MainMenu();
        }

        // Method to play greeting audio
        private void Greeting()
        {
            string full_location = AppDomain.CurrentDomain.BaseDirectory;
            string new_path = full_location.Replace("bin\\Debug\\", "");

            try
            {
                string full_path = Path.Combine(new_path, "greeting.wav");
                using (SoundPlayer play = new SoundPlayer(full_path))
                {
                    play.PlaySync();
                }
            }
            catch (Exception error)
            {
                Console.Write(error.Message);
            }
        }

        // Method to display the logo
        private void Logo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string paths = AppDomain.CurrentDomain.BaseDirectory;
            string new_path = paths.Replace("bin\\Debug\\", "");
            string full_path = Path.Combine(new_path, "chat.jpeg");

            try
            {
                Bitmap Logo = new Bitmap(full_path);
                Logo = new Bitmap(Logo, new Size(120, 50));

                for (int height = 0; height < Logo.Height; height++)
                {
                    for (int width = 0; width < Logo.Width; width++)
                    {
                        Color pixelColor = Logo.GetPixel(width, height);
                        int gray = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        char asciiChar = gray > 250 ? '.' : gray > 150 ? '*' : gray > 100 ? 'o' : gray > 50 ? '#' : '@';
                        Console.Write(asciiChar);
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception error)
            {
                Console.WriteLine($"Error loading logo: {error.Message}");
            }
            Console.ResetColor();
        }

        // Method to store user details
        private void userInfo()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=========================================================================================================");
            TypeWriterEffect("Chatbot AI -> Please enter your name:", 30);
            Console.WriteLine("=========================================================================================================");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("user -> ");
            userName = Console.ReadLine()?.Trim();

            while (string.IsNullOrWhiteSpace(userName) || !IsValidName(userName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                TypeWriterEffect("Chatbot AI -> Invalid input! Names should contain only letters and spaces.", 30);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("user -> ");
                userName = Console.ReadLine()?.Trim();
            }

            // Prompt for cybersecurity topic of interest
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=========================================================================================================");
            TypeWriterEffect("Chatbot AI -> Please enter a cybersecurity topic you're interested in (e.g., phishing, password safety, safe browsing, SQL injection, cybersecurity tips):", 30);
            Console.WriteLine("=========================================================================================================");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("user -> ");
            userInterest = Console.ReadLine()?.Trim().ToLower();

            while (string.IsNullOrWhiteSpace(userInterest))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                TypeWriterEffect("Chatbot AI -> Invalid input! Please enter a cybersecurity topic.", 30);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("user -> ");
                userInterest = Console.ReadLine()?.Trim().ToLower();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=========================================================================================================");
            TypeWriterEffect($"ChatBot AI -> I have saved your name as {userName} and your interest as {userInterest}", 30);
            Console.ResetColor();

            List<string> userData = new List<string>
            {
                $"New User Session: {DateTime.Now}",
                $"User Name: {userName}",
                $"User Interest: {userInterest}"
            };
            memoryManager.save_memory(userData);
            SaveConversation($"User entered name: {userName}");
            SaveConversation($"User entered interest: {userInterest}");

            if (!userInterests.Contains(userInterest))
            {
                userInterests.Add(userInterest);
                List<string> interestData = new List<string> { $"interest:{userInterest}" };
                memoryManager.save_memory(interestData);
            }
        }

        private bool IsValidName(string name)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z\s]+$");
        }

        // Method for the main menu
        private void MainMenu()
        {
            ClearConsole();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("===================================================================================================================");
            Console.WriteLine("Cybersecurity Awareness Chatbot");
            Console.WriteLine("===================================================================================================================");
            TypeWriterEffect($"Chatbot AI -> Welcome to the main menu {userName}", 30);
            TypeWriterEffect("ChatBot AI-> Pick an option (numbers only)", 30);
            TypeWriterEffect("1. View user details\n2. Ask ChatBot AI questions\n3. View conversation history\n4. Close", 30);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{userName} -> ");
            string option = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Green;

            bool validInput = false;
            while (!validInput)
            {
                switch (option)
                {
                    case "1":
                        TypeWriterEffect("ChatBot AI-> You have selected option: " + option, 30);
                        TypeWriterEffect($"Your full name is {userName}", 30);
                        TypeWriterEffect($"Your cybersecurity interest is {userInterest}", 30);
                        if (userInterests.Count > 0)
                            TypeWriterEffect($"Your interests are: {string.Join(", ", userInterests)}", 30);
                        validInput = true;
                        MainMenu();
                        break;
                    case "2":
                        TypeWriterEffect("ChatBot AI-> You have selected option: " + option, 30);
                        FilterandSort();
                        validInput = true;
                        MainMenu();
                        break;
                    case "3":
                        TypeWriterEffect("ChatBot AI-> You have selected option: " + option, 30);
                        ViewConversationHistory();
                        validInput = true;
                        MainMenu();
                        break;
                    case "4":
                        TypeWriterEffect("ChatBot AI-> You have selected option: " + option, 30);
                        TypeWriterEffect($"Goodbye {userName}! Stay safe online.", 30);
                        SaveConversation("User exited the application");
                        validInput = true;
                        Environment.Exit(0);
                        break;
                    default:
                        TypeWriterEffect($"ChatBot AI-> Please select a valid number {userName}!", 30);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($"{userName} -> ");
                        option = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                }
            }
        }

        // Method to clear console
        private void ClearConsole(string notice = "Clearing console", int timer = 5)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"\n {notice}");
            for (int i = timer; i > 0; i--)
            {
                Console.Write($"{i}... ");
                Thread.Sleep(1000);
            }
            Console.Clear();
        }

        // Method to filter and sort questions
        private void FilterandSort()
        {
            Store_ignore();
            Store_replies();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n=========================================================================================================");
                Console.WriteLine("Ask Away About Cybersecurity");
                Console.WriteLine("\n=========================================================================================================");
                TypeWriterEffect("ChatBot AI -> You can ask me about:", 30);
                Console.WriteLine("| 1. My purpose");
                Console.WriteLine("| 2. Password safety");
                Console.WriteLine("| 3. Phishing attacks");
                Console.WriteLine("| 4. Safe browsing");
                Console.WriteLine("| 5. SQL injection");
                Console.WriteLine("| 6. Cybersecurity tips");
                TypeWriterEffect("Enter your question (or type 'menu' to return to menu)", 30);
                Console.WriteLine("===========================================================================================================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{userName} -> ");
                string question = Console.ReadLine()?.Trim();

                while (string.IsNullOrWhiteSpace(question))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    TypeWriterEffect("Chatbot AI -> Invalid input! Please enter a question or 'menu' to return", 30);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"{userName} -> ");
                    question = Console.ReadLine()?.Trim();
                }

                Console.ForegroundColor = ConsoleColor.Green;

                if (question.ToLower() == "menu")
                {
                    SaveConversation($"User returned to menu after asking: {question}");
                    break;
                }

                SaveConversation($"User asked: {question}");
                string sentiment = DetectSentiment(question);

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("ChatBot is typing");
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(500);
                    Console.Write(".");
                }
                Console.WriteLine();
                Console.ResetColor();

                Thread.Sleep(_random.Next(1000, 3000));
                string response = GenerateResponse(question, sentiment, false);
                SaveConversation($"Bot replied: {response}");
                TypeWriterEffect($"ChatBot AI -> {response}", 40);

                // Prompt for follow-up
                Console.ForegroundColor = ConsoleColor.Green;
                TypeWriterEffect("ChatBot AI -> Would you like a follow-up on this topic? (yes/no)", 30);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{userName} -> ");
                string followUp = Console.ReadLine()?.Trim().ToLower();

                while (followUp != "yes" && followUp != "no")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    TypeWriterEffect("ChatBot AI -> Please enter 'yes' or 'no'.", 30);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"{userName} -> ");
                    followUp = Console.ReadLine()?.Trim().ToLower();
                }

                if (followUp == "yes")
                {
                    SaveConversation("User requested follow-up");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("ChatBot is typing");
                    for (int i = 0; i < 3; i++)
                    {
                        Thread.Sleep(500);
                        Console.Write(".");
                    }
                    Console.WriteLine();
                    Console.ResetColor();

                    Thread.Sleep(_random.Next(1000, 3000));
                    string followUpResponse = GenerateResponse(question, sentiment, true);
                    SaveConversation($"Bot follow-up: {followUpResponse}");
                    TypeWriterEffect($"ChatBot AI -> {followUpResponse}", 40);
                }
                else
                {
                    SaveConversation("User declined follow-up");
                }
            }
        }

        private string DetectSentiment(string input)
        {
            input = input.ToLower();
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("afraid") ||
                input.Contains("nervous") || input.Contains("anxious"))
                return "worried";
            if (input.Contains("angry") || input.Contains("mad") || input.Contains("frustrated") ||
                input.Contains("annoyed") || input.Contains("upset"))
                return "frustrated";
            if (input.Contains("happy") || input.Contains("excited") || input.Contains("glad") ||
                input.Contains("pleased") || input.Contains("thrilled"))
                return "happy";
            if (input.Contains("sad") || input.Contains("depressed") || input.Contains("unhappy") ||
                input.Contains("miserable") || input.Contains("down"))
                return "sad";
            if (input.Contains("interested") || input.Contains("curious") || input.Contains("want to know") ||
                input.Contains("wondering") || input.Contains("tell me about"))
                return "curious";
            return "neutral";
        }

        private string GenerateResponse(string question, string sentiment, bool isFollowUp)
        {
            // Track used responses for this question
            List<string> usedResponses = new List<string>();
            string currentTopic = null;

            // Determine the topic based on the question
            if (question.ToLower().Contains("password"))
                currentTopic = "password";
            else if (question.ToLower().Contains("phishing") || question.ToLower().Contains("scam"))
                currentTopic = "phishing";
            else if (question.ToLower().Contains("privacy") || question.ToLower().Contains("data protection"))
                currentTopic = "privacy";
            else if (question.ToLower().Contains("safe browsing") || question.ToLower().Contains("browsing"))
                currentTopic = "safe browsing";
            else if (question.ToLower().Contains("sql") || question.ToLower().Contains("injection"))
                currentTopic = "sql injection";
            else if (question.ToLower().Contains("cybersecurity") || question.ToLower().Contains("security tips"))
                currentTopic = "cybersecurity tips";
            else if (question.ToLower().Contains("your name") || question.ToLower().Contains("who are you"))
                currentTopic = "purpose";
            else if (question.ToLower().Contains("remember") || question.ToLower().Contains("what do you know"))
                currentTopic = "memory";
            else if (question.ToLower().Contains("interest") || question.ToLower().Contains("like") || question.ToLower().Contains("prefer"))
                currentTopic = "interests";
            else if (question.ToLower().Contains("history"))
                currentTopic = "history";

            // Check if the question matches the user's interest
            bool matchesInterest = !isFollowUp && userInterest != null && (
                (currentTopic == "password" && userInterest.Contains("password")) ||
                (currentTopic == "phishing" && userInterest.Contains("phishing")) ||
                (currentTopic == "safe browsing" && userInterest.Contains("browsing")) ||
                (currentTopic == "sql injection" && userInterest.Contains("sql")) ||
                (currentTopic == "cybersecurity tips" && userInterest.Contains("cybersecurity"))
            );

            ResponseHandler passwordHandler = (sent, followUp, used) =>
            {
                if (!userInterests.Contains("password"))
                {
                    userInterests.Add("password");
                    List<string> interestData = new List<string> { $"interest:password" };
                    memoryManager.save_memory(interestData);
                }
                var responses = new List<string>
                {
                    "password : Make sure to use strong, unique passwords for each account.",
                    "password : A good password should be at least 12 characters long and include numbers, symbols, and both uppercase and lowercase letters.",
                    "password : Consider using a password manager to keep track of your passwords securely.",
                    "password : Never share your passwords with anyone, even if they claim to be from tech support.",
                    "password : Enable two-factor authentication (2FA) to add an extra layer of security to your accounts.",
                    "password : Avoid using personal information like birthdays or names in your passwords.",
                    "password : Regularly update your passwords, especially after a data breach.",
                    "password : Use passphrases—longer phrases that are easier to remember but hard to crack."
                };
                if (followUp)
                {
                    responses.RemoveAll(r => used.Contains(r));
                    if (responses.Count == 0)
                        return AdjustForSentiment("I've shared all my tips on passwords. Try asking about another topic!", sent);
                }
                string selected = responses[_random.Next(responses.Count)];
                if (!followUp) used.Clear();
                used.Add(selected);
                string response = AdjustForSentiment(selected, sent);
                return matchesInterest && !followUp ? $"That's great, {userName}! You asked about {userInterest}, which matches your interest. Here's some advice: {response}" : response;
            };

            ResponseHandler phishingHandler = (sent, followUp, used) =>
            {
                if (!userInterests.Contains("phishing"))
                {
                    userInterests.Add("phishing");
                    List<string> interestData = new List<string> { $"interest:phishing" };
                    memoryManager.save_memory(interestData);
                }
                var responses = new List<string>
                {
                    "phishing : Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organizations.",
                    "phishing : Phishing emails often create a sense of urgency. Always verify before clicking links or providing information.",
                    "phishing : Check the sender's email address carefully—phishing attempts often use addresses that look similar to legitimate ones.",
                    "phishing : If an email seems suspicious, don't click any links. Instead, go directly to the company's website.",
                    "phishing : Look for spelling or grammar errors in emails, as these are common in phishing attempts.",
                    "phishing : Hover over links to see the actual URL before clicking—phishers often use fake links.",
                    "phishing : Be wary of unsolicited attachments, as they may contain malware.",
                    "phishing : Use email filters to block known phishing domains."
                };
                if (followUp)
                {
                    responses.RemoveAll(r => used.Contains(r));
                    if (responses.Count == 0)
                        return AdjustForSentiment("I've shared all my tips on phishing. Try asking about another topic!", sent);
                }
                string selected = responses[_random.Next(responses.Count)];
                if (!followUp) used.Clear();
                used.Add(selected);
                string response = AdjustForSentiment(selected, sent);
                return matchesInterest && !followUp ? $"That's great, {userName}! You asked about {userInterest}, which matches your interest. Here's some advice: {response}" : response;
            };

            ResponseHandler safeBrowsingHandler = (sent, followUp, used) =>
            {
                if (!userInterests.Contains("safe browsing"))
                {
                    userInterests.Add("safe browsing");
                    List<string> interestData = new List<string> { $"interest:safe browsing" };
                    memoryManager.save_memory(interestData);
                }
                var responses = new List<string>
                {
                    "safe browsing : Always look for the padlock icon and 'https://' in website addresses.",
                    "safe browsing : Keep your browser updated and avoid downloading files from untrusted sources.",
                    "safe browsing : Avoid entering personal information on untrusted or unknown websites.",
                    "safe browsing : Use browser security features like pop-up blockers and safe browsing modes.",
                    "safe browsing : Clear your browser cache regularly to remove stored sensitive data.",
                    "safe browsing : Be cautious of free Wi-Fi networks—use a VPN for secure browsing.",
                    "safe browsing : Disable autofill for sensitive information like credit card details.",
                    "safe browsing : Check website reviews or ratings before making online purchases."
                };
                if (followUp)
                {
                    responses.RemoveAll(r => used.Contains(r));
                    if (responses.Count == 0)
                        return AdjustForSentiment("I've shared all my tips on safe browsing. Try asking about another topic!", sent);
                }
                string selected = responses[_random.Next(responses.Count)];
                if (!followUp) used.Clear();
                used.Add(selected);
                string response = AdjustForSentiment(selected, sent);
                return matchesInterest && !followUp ? $"That's great, {userName}! You asked about {userInterest}, which matches your interest. Here's some advice: {response}" : response;
            };

            ResponseHandler sqlInjectionHandler = (sent, followUp, used) =>
            {
                if (!userInterests.Contains("sql injection"))
                {
                    userInterests.Add("sql injection");
                    List<string> interestData = new List<string> { $"interest:sql injection" };
                    memoryManager.save_memory(interestData);
                }
                var responses = new List<string>
                {
                    "sql injection : SQL injection involves attackers inserting malicious SQL code into input fields to manipulate databases.",
                    "sql injection : Use parameterized queries or prepared statements to prevent SQL injection attacks.",
                    "sql injection : Validate and sanitize all user inputs to ensure they don't contain malicious code.",
                    "sql injection : Avoid displaying detailed error messages that could reveal database structure to attackers.",
                    "sql injection : Regularly update your database software to patch known vulnerabilities.",
                    "sql injection : Use an ORM (Object-Relational Mapping) tool to reduce the risk of SQL injection.",
                    "sql injection : Restrict database user permissions to limit the impact of a successful attack.",
                    "sql injection : Monitor and log database queries to detect suspicious activity."
                };
                if (followUp)
                {
                    responses.RemoveAll(r => used.Contains(r));
                    if (responses.Count == 0)
                        return AdjustForSentiment("I've shared all my tips on SQL injection. Try asking about another topic!", sent);
                }
                string selected = responses[_random.Next(responses.Count)];
                if (!followUp) used.Clear();
                used.Add(selected);
                string response = AdjustForSentiment(selected, sent);
                return matchesInterest && !followUp ? $"That's great, {userName}! You asked about {userInterest}, which matches your interest. Here's some advice: {response}" : response;
            };

            ResponseHandler cybersecurityTipsHandler = (sent, followUp, used) =>
            {
                if (!userInterests.Contains("cybersecurity tips"))
                {
                    userInterests.Add("cybersecurity tips");
                    List<string> interestData = new List<string> { $"interest:cybersecurity tips" };
                    memoryManager.save_memory(interestData);
                }
                var responses = new List<string>
                {
                    "cybersecurity tips : Keep all software, including operating systems and apps, up to date with the latest security patches.",
                    "cybersecurity tips : Use antivirus software and keep it updated to protect against malware.",
                    "cybersecurity tips : Be cautious about sharing personal information on social media—it can be used by attackers.",
                    "cybersecurity tips : Regularly back up important data to an external drive or cloud service.",
                    "cybersecurity tips : Learn to recognize social engineering tactics, like pretexting or baiting.",
                    "cybersecurity tips : Use strong, unique passwords and enable 2FA wherever possible.",
                    "cybersecurity tips : Avoid using public computers for sensitive tasks like online banking.",
                    "cybersecurity tips : Educate yourself about the latest cybersecurity threats and best practices."
                };
                if (followUp)
                {
                    responses.RemoveAll(r => used.Contains(r));
                    if (responses.Count == 0)
                        return AdjustForSentiment("I've shared all my cybersecurity tips. Try asking about another topic!", sent);
                }
                string selected = responses[_random.Next(responses.Count)];
                if (!followUp) used.Clear();
                used.Add(selected);
                string response = AdjustForSentiment(selected, sent);
                return matchesInterest && !followUp ? $"That's great, {userName}! You asked about {userInterest}, which matches your interest. Here's some advice: {response}" : response;
            };

            var responseHandlers = new Dictionary<QuestionMatcher, ResponseHandler>
            {
                { q => q.Contains("password"), passwordHandler },
                { q => q.Contains("phishing") || q.Contains("scam"), phishingHandler },
                { q => q.Contains("safe browsing") || q.Contains("browsing"), safeBrowsingHandler },
                { q => q.Contains("sql") || q.Contains("injection"), sqlInjectionHandler },
                { q => q.Contains("cybersecurity") || q.Contains("security tips"), cybersecurityTipsHandler },
                { q => q.Contains("privacy") || q.Contains("data protection"), (sent, followUp, used) =>
                    {
                        if (!userInterests.Contains("privacy"))
                        {
                            userInterests.Add("privacy");
                            List<string> interestData = new List<string> { $"interest:privacy" };
                            memoryManager.save_memory(interestData);
                        }
                        var responses = new List<string>
                        {
                            "privacy : Review privacy settings on your social media accounts regularly to control what information is shared.",
                            "privacy : Be careful about what personal information you share online—once it's out there, it's hard to take back.",
                            "privacy : Use privacy-focused browsers and search engines to minimize tracking of your online activities.",
                            "privacy : Consider using a VPN to protect your online privacy, especially on public Wi-Fi networks."
                        };
                        if (followUp)
                        {
                            responses.RemoveAll(r => used.Contains(r));
                            if (responses.Count == 0)
                                return AdjustForSentiment("I've shared all my tips on privacy. Try asking about another topic!", sent);
                        }
                        string selected = responses[_random.Next(responses.Count)];
                        if (!followUp) used.Clear();
                        used.Add(selected);
                        return AdjustForSentiment(selected, sent);
                    }
                },
                { q => q.Contains("your name") || q.Contains("who are you"), (sent, followUp, used) =>
                    {
                        string selected = "I'm your Cybersecurity Awareness Chatbot, here to help you stay safe online!";
                        if (!followUp) used.Clear();
                        used.Add(selected);
                        return AdjustForSentiment(followUp ? "I'm the same Cybersecurity Chatbot, ready to answer more of your questions!" : selected, sent);
                    }
                },
                { q => q.Contains("remember") || q.Contains("what do you know"), (sent, followUp, used) =>
                    {
                        string selected = RecallUserInfo();
                        if (!followUp) used.Clear();
                        used.Add(selected);
                        return AdjustForSentiment(followUp ? "I still remember your interests! Want to dive deeper into one of them?" : selected, sent);
                    }
                },
                { q => q.Contains("interest") || q.Contains("like") || q.Contains("prefer"), (sent, followUp, used) =>
                    {
                        string selected = userInterests.Count > 0
                            ? $"Based on our conversation, you seem interested in: {string.Join(", ", userInterests)}..."
                            : "You haven't mentioned any specific interests yet...";
                        if (!followUp) used.Clear();
                        used.Add(selected);
                        return AdjustForSentiment(followUp ? "Your interests help me tailor my advice. Want more details on any of these?" : selected, sent);
                    }
                },
                { q => q.Contains("history"), (sent, followUp, used) =>
                    {
                        string selected = "You can type 'history' in the main menu to see our conversation history.";
                        if (!followUp) used.Clear();
                        used.Add(selected);
                        return AdjustForSentiment(followUp ? "Our conversation history is stored safely. Check it out from the main menu!" : selected, sent);
                    }
                }
            };

            foreach (var handler in responseHandlers)
            {
                if (handler.Key(question.ToLower()))
                {
                    return handler.Value(sentiment, isFollowUp, usedResponses);
                }
            }

            // Keyword-based replies from the replies list
            string[] store_word = question.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            ArrayList store_final_words = new ArrayList();

            for (int count = 0; count < store_word.Length; count++)
            {
                if (!ignore.Contains(store_word[count]))
                {
                    store_final_words.Add(store_word[count]);
                }
            }

            List<string> matchingReplies = new List<string>();
            if (currentTopic != null)
            {
                // Filter replies to only those matching the topic
                matchingReplies = replies.Where(r => r.StartsWith(currentTopic + " :")).ToList();
            }
            else
            {
                // Fallback to keyword matching if no specific topic
                for (int counting = 0; counting < store_final_words.Count; counting++)
                {
                    string currentWord = store_final_words[counting].ToString();
                    for (int count = 0; count < replies.Count; count++)
                    {
                        string currentReply = replies[count];
                        if (currentReply.IndexOf(currentWord, StringComparison.OrdinalIgnoreCase) >= 0 && !matchingReplies.Contains(currentReply))
                        {
                            matchingReplies.Add(currentReply);
                        }
                    }
                }
            }

            if (matchingReplies.Count > 0)
            {
                if (isFollowUp)
                {
                    matchingReplies.RemoveAll(r => usedResponses.Contains(r));
                    if (matchingReplies.Count == 0)
                        return AdjustForSentiment($"I've shared all my tips on {currentTopic ?? "this topic"}. Try asking about another topic!", sentiment);
                }
                string selected = matchingReplies[_random.Next(matchingReplies.Count)];
                if (!isFollowUp) usedResponses.Clear();
                usedResponses.Add(selected);
                string response = AdjustForSentiment(selected, sentiment);
                return matchesInterest && !isFollowUp ? $"That's great, {userName}! You asked about {userInterest}, which matches your interest. Here's some advice: {response}" : response;
            }

            string defaultResponse = "I'm sorry, I don't understand that question. Please ask about cybersecurity topics.";
            if (!isFollowUp) usedResponses.Clear();
            usedResponses.Add(defaultResponse);
            return AdjustForSentiment(isFollowUp ? "I didn't catch that question earlier. Could you rephrase it or try a different topic?" : defaultResponse, sentiment);
        }

        private string AdjustForSentiment(string response, string sentiment)
        {
            switch (sentiment)
            {
                case "worried":
                    return "I understand this might be concerning. " + response + " Remember, being aware is the first step to staying safe.";
                case "frustrated":
                    return "I hear your frustration. Cybersecurity can be complex, but " + response.ToLower();
                case "happy":
                    return "Great to see your enthusiasm! " + response;
                case "sad":
                    return "I'm sorry you're feeling this way. " + response + " Taking small steps can help improve your security.";
                case "curious":
                    return "That's a great question! " + response;
                default:
                    return response;
            }
        }

        private string RecallUserInfo()
        {
            List<string> memory = memoryManager.return_memory();
            List<string> interests = new List<string>();

            foreach (string line in memory)
            {
                if (line.StartsWith("interest:"))
                {
                    interests.Add(line.Substring("interest:".Length));
                }
            }

            return interests.Count > 0
                ? $"I remember you're interested in {string.Join(" and ", interests)}. Would you like to know more about these topics?"
                : "I don't have any specific information stored about your interests yet.";
        }

        // Method to store replies
        private void Store_replies()
        {
            // Password safety
            replies.Add("password : Always use a mix of uppercase, lowercase, numbers, and symbols!");
            replies.Add("password : Never reuse passwords across different accounts.");
            replies.Add("password : Consider using a password manager to keep track of your passwords securely.");
            replies.Add("password : Never share your passwords with anyone, even if they claim to be from tech support.");
            replies.Add("password : Enable two-factor authentication (2FA) to add an extra layer of security to your accounts.");
            replies.Add("password : Avoid using personal information like birthdays or names in your passwords.");
            replies.Add("password : Regularly update your passwords, especially after a data breach.");
            replies.Add("password : Use passphrases—longer phrases that are easier to remember but hard to crack.");

            // Phishing
            replies.Add("phishing : Attacks often come through emails pretending to be from trusted sources—always verify sender addresses.");
            replies.Add("phishing : Watch for urgent language in messages asking for personal info—this is a common scam tactic.");
            replies.Add("phishing : Check the sender's email address carefully—phishing attempts often use addresses that look similar to legitimate ones.");
            replies.Add("phishing : If an email seems suspicious, don't click any links. Instead, go directly to the company's website.");
            replies.Add("phishing : Look for spelling or grammar errors in emails, as these are common in phishing attempts.");
            replies.Add("phishing : Hover over links to see the actual URL before clicking—phishers often use fake links.");
            replies.Add("phishing : Be wary of unsolicited attachments, as they may contain malware.");
            replies.Add("phishing : Use email filters to block known phishing domains.");

            // Safe browsing
            replies.Add("safe browsing : Always look for the padlock icon and 'https://' in website addresses.");
            replies.Add("safe browsing : Keep your browser updated and avoid downloading files from untrusted sources.");
            replies.Add("safe browsing : Avoid entering personal information on untrusted or unknown websites.");
            replies.Add("safe browsing : Use browser security features like pop-up blockers and safe browsing modes.");
            replies.Add("safe browsing : Clear your browser cache regularly to remove stored sensitive data.");
            replies.Add("safe browsing : Be cautious of free Wi-Fi networks—use a VPN for secure browsing.");
            replies.Add("safe browsing : Disable autofill for sensitive information like credit card details.");
            replies.Add("safe browsing : Check website reviews or ratings before making online purchases.");

            // SQL injection
            replies.Add("sql injection : SQL injection involves attackers inserting malicious SQL code into input fields to manipulate databases.");
            replies.Add("sql injection : Use parameterized queries or prepared statements to prevent SQL injection attacks.");
            replies.Add("sql injection : Validate and sanitize all user inputs to ensure they don't contain malicious code.");
            replies.Add("sql injection : Avoid displaying detailed error messages that could reveal database structure to attackers.");
            replies.Add("sql injection : Regularly update your database software to patch known vulnerabilities.");
            replies.Add("sql injection : Use an ORM (Object-Relational Mapping) tool to reduce the risk of SQL injection.");
            replies.Add("sql injection : Restrict database user permissions to limit the impact of a successful attack.");
            replies.Add("sql injection : Monitor and log database queries to detect suspicious activity.");

            // Cybersecurity tips
            replies.Add("cybersecurity tips : Keep all software, including operating systems and apps, up to date with the latest security patches.");
            replies.Add("cybersecurity tips : Use antivirus software and keep it updated to protect against malware.");
            replies.Add("cybersecurity tips : Be cautious about sharing personal information on social media—it can be used by attackers.");
            replies.Add("cybersecurity tips : Regularly back up important data to an external drive or cloud service.");
            replies.Add("cybersecurity tips : Learn to recognize social engineering tactics, like pretexting or baiting.");
            replies.Add("cybersecurity tips : Use strong, unique passwords and enable 2FA wherever possible.");
            replies.Add("cybersecurity tips : Avoid using public computers for sensitive tasks like online banking.");
            replies.Add("cybersecurity tips : Educate yourself about the latest cybersecurity threats and best practices.");

            // Other topics
            replies.Add("purpose : I'm a cybersecurity chatbot designed to help you understand online threats and protection methods.");
            replies.Add("purpose : Is to provide basic cybersecurity awareness and answer your related questions.");
            replies.Add("hello : I'm an AI assistant, so I don't have feelings, but I'm ready to help with cybersecurity questions!");
            replies.Add("ransomware : Encrypts files and demands payment for their release.");
            replies.Add("malware : Includes viruses, worms, and trojans that harm computer systems.");
            replies.Add("firewall : Protects networks by filtering incoming and outgoing traffic.");
            replies.Add("encryption : Scrambles data to make it unreadable without the proper key.");
            replies.Add("vpn : Creates secure connections over public networks.");
            replies.Add("backup : Regular backups protect against data loss from attacks or failures.");
        }

        // Method to store ignored words
        private void Store_ignore()
        {
            ignore.Add("tell");
            ignore.Add("me");
            ignore.Add("about");
            ignore.Add("are");
            ignore.Add("you");
            ignore.Add("your");
            ignore.Add("what's");
            ignore.Add("can");
            ignore.Add("i");
            ignore.Add("ask");
            ignore.Add("safety");
            ignore.Add("safe");
            ignore.Add("and");
            ignore.Add("also");
            ignore.Add("with");
            ignore.Add("together");
            ignore.Add("the");
            ignore.Add("a");
            ignore.Add("an");
            ignore.Add("how");
            ignore.Add("what");
            ignore.Add("where");
            ignore.Add("when");
            ignore.Add("why");
        }

        // Method for typewriter effect
        private void TypeWriterEffect(string text, int delayMs = 50)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            for (int i = 0; i < text.Length; i++)
            {
                Console.Write(text[i]);
                Thread.Sleep(delayMs);
                if (new[] { '.', ',', '!', '?' }.Contains(text[i]))
                {
                    Thread.Sleep(_random.Next(100, 300));
                }
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        // Method to save conversation
        private void SaveConversation(string message)
        {
            List<string> conversation = new List<string>
            {
                $"[{DateTime.Now:HH:mm:ss}] {message}"
            };
            memoryManager.save_memory(conversation);
        }

        // Method to view conversation history
        private void ViewConversationHistory()
        {
            List<string> history = memoryManager.return_memory();
            ClearConsole();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n====================================================================");
            TypeWriterEffect("ChatBot AI-> Conversation History:", 30);
            Console.WriteLine("====================================================================");

            if (history.Count == 0)
            {
                TypeWriterEffect("No conversation history found.", 30);
            }
            else
            {
                foreach (string entry in history)
                {
                    if (!entry.StartsWith("New User Session:") && !entry.StartsWith("User Name:") && !entry.StartsWith("interest:") && !entry.StartsWith("User Interest:"))
                    {
                        TypeWriterEffect(entry, 20);
                    }
                }
            }

            Console.WriteLine("====================================================================");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}