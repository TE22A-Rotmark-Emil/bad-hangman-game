using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

static bool ValidGuess(char guess, List<char> previousGuesses){
    char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

    if (alphabet.Contains(guess) == false){
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(guess + " is not a valid English language character.");
        Console.ForegroundColor = ConsoleColor.Gray;
        return false;
    }

    for (int i = 0; i < previousGuesses.Count(); i++)
    {
        if (guess == previousGuesses[i]){
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(guess + " has already been guessed. Try a different character.");
            Console.ForegroundColor = ConsoleColor.Gray;
            return false;
        }
    }
    return true;
}

static int DifficultySelector(){
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Which difficulty would you like?");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("1. Easy");
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("2. Medium");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("3. Hard");
    Console.ForegroundColor = ConsoleColor.Gray;

    bool answered = false;
    int answer = 1;
    string userAnswer;
    string[] permittedWords = ["1", "2", "3", "easy", "medium", "hard"];
    do{
        userAnswer = Console.ReadLine();
        userAnswer = userAnswer.Replace(" ", "");
        userAnswer = userAnswer.Replace(".", "");
        userAnswer = userAnswer.ToLower();
        foreach (string word in permittedWords){
            if (userAnswer == word){
                if (userAnswer == "easy"){userAnswer = "1";}
                else if (userAnswer == "medium"){userAnswer = "2";}
                else if (userAnswer == "hard"){userAnswer = "3";}
                int.TryParse(userAnswer, out answer);
                answered = true;
                break;
            }
        }
        if (answered == false){
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("This is an invalid answer.");
            Console.WriteLine("Answer with either the number assigned to each difficulty, or with the name of the difficulty itself.");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }while(answered == false);

    return answer;
}

static int CalculateErrorPermission(string word, int difficulty){
    char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    char[] wordLetters = word.ToCharArray();
    int numberOfUniqueLetters = 0;
    for (int i = 0; i < alphabet.Count(); i++){
        foreach (char letter in wordLetters){
            char wordLetter = char.ToLower(letter);
            if (wordLetter == alphabet[i]){
                numberOfUniqueLetters++;
                break;
            }
        }
    }
    int permittableErrorCount = (alphabet.Count()-numberOfUniqueLetters)/3;
    if (permittableErrorCount < 24 && difficulty == 1){
        permittableErrorCount++;
    }
    else if (difficulty == 3 && Convert.ToInt32(permittableErrorCount/2) != 0){
        permittableErrorCount = Convert.ToInt32(permittableErrorCount/2);
    }
    return permittableErrorCount;
}

static void Commands(string environment){

}

int difficulty = DifficultySelector();

bool wordSolved = false;
List<string> words = ["Append", "Terrible", "Cause"];
int randomChoice = Random.Shared.Next(words.Count());
string word = words[randomChoice];
List<char> lettersInTheWord = new();
List<char> solvedLettersInTheWord = new();
List<char> guesses = new();
int numberOfPermittedFailures = CalculateErrorPermission(word, difficulty);
int numberOfFailures = 0;
int charAppearInWord = 0;
char guess = 'a';

foreach (char letter in word){
    lettersInTheWord.Add(letter);
    solvedLettersInTheWord.Add('_');
}

while (wordSolved == false){
    if (numberOfFailures == 0 && charAppearInWord == 0){
        Console.WriteLine($"You may fail up to {numberOfPermittedFailures} times.");
    }
    else if (numberOfFailures > 0){
        Console.Write("You may fail " + (numberOfPermittedFailures-numberOfFailures) + " more time");
        if (numberOfPermittedFailures-numberOfFailures != 1){
            Console.Write("s");
        }
        Console.WriteLine(".");
    }
    
    charAppearInWord = 0;

    for (int i = 0; i < lettersInTheWord.Count(); i++)
    {
        if (i < solvedLettersInTheWord.Count()-1){
            Console.Write(solvedLettersInTheWord[i] + " ");
        }
        else{
            Console.WriteLine(solvedLettersInTheWord[i] + $" ({i+1})");
        }
    }

    bool validAnswer;
    string playerRealGuess;
    string guessedWord = "";
    do{
        Console.WriteLine("guess");
        playerRealGuess = Console.ReadLine();
        validAnswer = char.TryParse(playerRealGuess, out guess);
        Console.WriteLine();
        if (validAnswer == false && guess != 'a'){
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Guessing a word suggests knowledge of the word. If you believe your answer is correct, re-type this word.");
            Console.WriteLine("If you are incorrect, you will immediately lose the game.");
            Console.ForegroundColor = ConsoleColor.Gray;
            guessedWord = playerRealGuess; // fix this
        } else if (validAnswer == false && guessedWord.Count() > 0){
            if (playerRealGuess.ToLower() == guessedWord.ToLower()){
                wordSolved = true;
                break;
            }
            else{
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Incorrect word.");
                Console.ForegroundColor = ConsoleColor.Gray;
                numberOfFailures = numberOfPermittedFailures-1;
                break;
            }
        } else{
            guess = char.ToLower(guess);
            validAnswer = ValidGuess(guess, guesses);
        }
    } while(validAnswer == false);
    guesses.Add(guess);

    for (int i = 0; i < lettersInTheWord.Count(); i++)
    {
        char inLetter = char.ToLower(lettersInTheWord[i]);
        if (inLetter == guess){
            charAppearInWord++;
        }
    }

    if (charAppearInWord > 0){
        for (int i = 0; i < lettersInTheWord.Count; i++)
        {
            char inWord = char.ToLower(lettersInTheWord[i]);
            if (guess == inWord){
                solvedLettersInTheWord[i] = lettersInTheWord[i];
            }
        }
    } else{
        numberOfFailures++;
    }

    int numberOfMatches = 0;
    for (int i = 0; i < lettersInTheWord.Count(); i++)
    {
        if (lettersInTheWord[i] == solvedLettersInTheWord[i]){
            numberOfMatches++;
        }
    }

    if (numberOfMatches == lettersInTheWord.Count()){
        wordSolved = true;
    } else if (numberOfFailures == numberOfPermittedFailures){
        break;
    }

}

if (wordSolved == true){
    Console.WriteLine("You Win! You get 1 000 000 000 dollars");
    Console.ReadLine();
} else{
    Console.WriteLine("Bevel in your disparity, loser");
    Console.ReadLine();
}