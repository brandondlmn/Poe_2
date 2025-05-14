# Poe_2
Project Details
Project Name:  Chatbot_Poe_Part2

Cybersecurity Awareness Chatbot
Overview
A C# console application that educates users about cybersecurity through an interactive chatbot interface. The chatbot covers topics like phishing, password safety, and secure browsing with a user-friendly experience.

Key Features
Interactive Menu System: View details, ask questions, or see history

Smart Conversations:

Ask follow-up questions on the same topic

Detects user sentiment (worried, curious, etc.)

Remembers conversation history

Engaging Interface:

ASCII art welcome screen

Audio greeting

Typewriter-style message display

Visual borders around messages

Quick Start
Requirements: .NET Framework 4.7.2+

Setup:

bash
git clone <repository-url>
cd Chatbot_Poe_Part2
Run: Execute the compiled program

How to Use
Enter your name and a cybersecurity interest when prompted

Choose from the main menu:

1: View your details

2: Ask cybersecurity questions

3: View conversation history

4: Exit

Example Interaction
==================================================
ChatBot AI -> What is phishing?
==================================================
phishing : Be cautious of emails asking for personal information...

Would you like a follow-up? (yes/no)
> yes

phishing : Also check sender email addresses carefully...
Troubleshooting
If audio/graphics don't load:

Ensure greeting.wav and chat.jpeg are in the program folder

Install System.Drawing.Common via NuGet if needed

Extending the Bot
Add new responses by editing the replies list in code:

csharp
replies.Add("new topic : Example response text");
