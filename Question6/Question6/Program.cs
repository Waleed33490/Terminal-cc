using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter usernames (separated by commas): ");
        string input = Console.ReadLine();
        string[] usernames = input.Split(',');

        List<string> invalidUsernames = new List<string>();
        List<User> validUsernames = new List<User>();

        foreach (string username in usernames)
        {
            string trimmedUsername = username.Trim();
            if (ValidateUsername(trimmedUsername, out string validationMessage, out User user))
            {
                validUsernames.Add(user);
                Console.WriteLine($"{trimmedUsername} - Valid");
                Console.WriteLine($"Letters: {user.LetterCount} (Uppercase: {user.UppercaseCount}, Lowercase: {user.LowercaseCount}), Digits: {user.DigitCount}, Underscores: {user.UnderscoreCount}");
                string password = GeneratePassword();
                string passwordStrength = CheckPasswordStrength(password);
                Console.WriteLine($"Generated Password: {password} (Strength: {passwordStrength})");
                Console.WriteLine();
            }
            else
            {
                invalidUsernames.Add(trimmedUsername);
                Console.WriteLine($"{trimmedUsername} - Invalid ({validationMessage})\n");
            }
        }

        Console.WriteLine("Summary:");
        Console.WriteLine($"- Total Usernames: {usernames.Length}");
        Console.WriteLine($"- Valid Usernames: {validUsernames.Count}");
        Console.WriteLine($"- Invalid Usernames: {invalidUsernames.Count}");
        Console.WriteLine();

        if (invalidUsernames.Count > 0)
        {
            Console.WriteLine("Invalid Usernames:");
            foreach (string invalid in invalidUsernames)
            {
                Console.WriteLine(invalid);
            }
            Console.WriteLine("Do you want to retry invalid usernames? (y/n): ");
            if (Console.ReadLine().ToLower() == "y")
            {
                Console.WriteLine("Enter invalid usernames: ");
                string retryInput = Console.ReadLine();
                string[] retryUsernames = retryInput.Split(',');

                foreach (string retry in retryUsernames)
                {
                    string trimmedRetry = retry.Trim();
                    if (ValidateUsername(trimmedRetry, out string retryMessage, out User retryUser))
                    {
                        validUsernames.Add(retryUser);
                        Console.WriteLine($"{trimmedRetry} - Valid");
                        string password = GeneratePassword();
                        string passwordStrength = CheckPasswordStrength(password);
                        Console.WriteLine($"Generated Password: {password} (Strength: {passwordStrength})");
                    }
                    else
                    {
                        Console.WriteLine($"{trimmedRetry} - Invalid ({retryMessage})");
                    }
                }
            }
        }

        SaveResultsToFile(validUsernames, invalidUsernames);
    }

    static bool ValidateUsername(string username, out string validationMessage, out User user)
    {
        user = new User { Username = username };
        if (username.Length < 5 || username.Length > 15)
        {
            validationMessage = "Username length must be between 5 and 15";
            return false;
        }
        if (!char.IsLetter(username[0]))
        {
            validationMessage = "Username must start with a letter";
            return false;
        }
        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
        {
            validationMessage = "Username must only contain letters, numbers, and underscores";
            return false;
        }

        user.UppercaseCount = 0;
        user.LowercaseCount = 0;
        user.DigitCount = 0;
        user.UnderscoreCount = 0;

        foreach (char ch in username)
        {
            if (char.IsUpper(ch)) user.UppercaseCount++;
            else if (char.IsLower(ch)) user.LowercaseCount++;
            else if (char.IsDigit(ch)) user.DigitCount++;
            else if (ch == '_') user.UnderscoreCount++;
        }

        user.LetterCount = user.UppercaseCount + user.LowercaseCount;
        validationMessage = string.Empty;
        return true;
    }

    static string GeneratePassword()
    {
        Random rand = new Random();
        string password = "";
        string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        string digits = "0123456789";
        string specialChars = "!@#$%^&*";

        password += GetRandomCharacters(upperChars, 2, rand);
        password += GetRandomCharacters(lowerChars, 2, rand);
        password += GetRandomCharacters(digits, 2, rand);
        password += GetRandomCharacters(specialChars, 2, rand);

        string allChars = upperChars + lowerChars + digits + specialChars;
        password += GetRandomCharacters(allChars, 6, rand);

        char[] passwordArray = password.ToCharArray();
        for (int i = 0; i < passwordArray.Length; i++)
        {
            int j = rand.Next(i, passwordArray.Length);
            char temp = passwordArray[i];
            passwordArray[i] = passwordArray[j];
            passwordArray[j] = temp;
        }

        return new string(passwordArray);
    }

    static string GetRandomCharacters(string chars, int count, Random rand)
    {
        string result = "";
        for (int i = 0; i < count; i++)
        {
            result += chars[rand.Next(chars.Length)];
        }
        return result;
    }

    static string CheckPasswordStrength(string password)
    {
        int score = 0;
        if (password.Length >= 12) score++;
        if (Regex.IsMatch(password, @"[A-Z]")) score++;
        if (Regex.IsMatch(password, @"[a-z]")) score++;
        if (Regex.IsMatch(password, @"[0-9]")) score++;
        if (Regex.IsMatch(password, @"[!@#$%^&*]")) score++;

        if (score == 5) return "Strong";
        if (score == 4) return "Medium";
        return "Weak";
    }

    static void SaveResultsToFile(List<User> validUsernames, List<string> invalidUsernames)
    {
        string filePath = "C:\\Users\\abdul\\source\\repos\\LabFinalCC\\UserDetails.txt";
        Console.WriteLine(filePath);
        using (StreamWriter sw = new StreamWriter(filePath))
        {
            int totalUsernames = validUsernames.Count + invalidUsernames.Count;
            sw.WriteLine("Validation Results:");

            foreach (var user in validUsernames)
            {
                sw.WriteLine($"{user.Username} - Valid");
                sw.WriteLine($"Letters: {user.LetterCount} (Uppercase: {user.UppercaseCount}, Lowercase: {user.LowercaseCount}), Digits: {user.DigitCount}, Underscores: {user.UnderscoreCount}");
                string password = GeneratePassword();
                string passwordStrength = CheckPasswordStrength(password);
                sw.WriteLine($"Generated Password: {password} (Strength: {passwordStrength})");
                sw.WriteLine();
            }

            foreach (string invalid in invalidUsernames)
            {
                sw.WriteLine($"{invalid} - Invalid");
            }

            sw.WriteLine();
            sw.WriteLine($"Summary:");
            sw.WriteLine($"- Total Usernames: {totalUsernames}");
            sw.WriteLine($"- Valid Usernames: {validUsernames.Count}");
            sw.WriteLine($"- Invalid Usernames: {invalidUsernames.Count}");
        }
    }

}

class User
{
    public string Username { get; set; }
    public int UppercaseCount { get; set; }
    public int LowercaseCount { get; set; }
    public int DigitCount { get; set; }
    public int UnderscoreCount { get; set; }
    public int LetterCount { get; set; }
}