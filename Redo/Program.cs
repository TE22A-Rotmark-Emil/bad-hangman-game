using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

#region methods
/* Checks for if the guessed letter is appropriate
(if it has already been guessed, if it is a valid letter) */
static bool ValidGuess(char guess, List<char> previousGuesses){
    char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

    if (alphabet.Contains(guess) == false){ // checks if letter is in the English alphabet
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(guess + " is not a valid English language character.");
        Console.ForegroundColor = ConsoleColor.Gray;
        return false;
    }
    if (previousGuesses.Contains(guess)){ // checks if letter has already been guessed
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(guess + " has already been guessed. Try a different character.");
        Console.ForegroundColor = ConsoleColor.Gray;
        return false;
    }
    return true; // if all other checks fail, the guess is valid
}

/* Decides the difficulty of the game, reducing the number of permittable failed guesses
and additionally forces a harder word selection */
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
    string[] permittedWords = ["1", "2", "3", "easy", "medium", "hard"]; // allows word and number
    while(answered == false){ // always runs once since answered is initiated as false
        userAnswer = Console.ReadLine();
        userAnswer = userAnswer.Replace(" ", ""); // following three lines trims the text from user error
        userAnswer = userAnswer.Replace(".", "");
        userAnswer = userAnswer.ToLower();
        foreach (string word in permittedWords){
            if (userAnswer == word){ // checks for if the word typed exists in the accepted list of words
                if (userAnswer == "easy"){userAnswer = "1";} // following three lines converts text answers to number answers
                else if (userAnswer == "medium"){userAnswer = "2";}
                else if (userAnswer == "hard"){userAnswer = "3";}
                int.TryParse(userAnswer, out answer); // makes it work as output, which is an int
                answered = true;
                break; // exists loop since only one answer is possible, making subsequent loops worthless
            }
        }
        if (answered == false){ // runs if the user typed an invalid output
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("This is an invalid answer.");
            Console.WriteLine("Answer with either the number assigned to each difficulty, or with the name of the difficulty itself.");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

    return answer;
}

/* Allows for flexible words and arbitrary letter counts to not mess with the error calculation.
For instance, if a player gets a word which has 25 distinct characters, the player would only
be able to lose the game if they write the 26th, unaccounted for, letter, however, if a player
instead gets a word with only 1 letter, they'd have 25 chances to pick the wrong answer. The
code as written below is created to always allow for failure, as well as vary difficulty
depending on the player's choice in the DifficultySelector method. */
static int CalculateErrorPermission(string word, int difficulty){
    char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    char[] wordLetters = word.ToCharArray();
    int numberOfUniqueLetters = 0;
    for (int i = 0; i < alphabet.Count(); i++){ // calculates how many separate letters exist in the chosen word
        foreach (char letter in wordLetters){
            char wordLetter = char.ToLower(letter);
            if (wordLetter == alphabet[i]){
                numberOfUniqueLetters++;
                break;
            }
        }
    }
    int permittableErrorCount = (alphabet.Count()-numberOfUniqueLetters)/3; // varies how many wrong guesses may be executed
    if (permittableErrorCount < 24 && difficulty == 1){ // both if statements vary difficulty depending on choice
        permittableErrorCount++;
    }
    else if (difficulty == 3 && Convert.ToInt32(permittableErrorCount/2) != 0){ // guarantees the player does not automatically lose
        permittableErrorCount = Convert.ToInt32(permittableErrorCount/2);
    }
    return permittableErrorCount;
}

static void Commands(string environment){

}
#endregion

#region variables
int difficulty = DifficultySelector(); // calls difficulty selector, which picks a number 1-3
bool wordSolved = false;
List<string> easyOrMediumWords = ["Append", "Terrible", "Cause"];
List<string> hardWords = ["Juxtaposition", "Phlegm", "Asphyxiation"];
List<string> words;
if (difficulty == 1 || difficulty == 2){words = easyOrMediumWords;} else{words = hardWords;}
int randomChoice = Random.Shared.Next(words.Count());
string word = words[randomChoice];
List<char> lettersInTheWord = new();
List<char> solvedLettersInTheWord = new();
List<char> guesses = new();
int numberOfPermittedFailures = CalculateErrorPermission(word, difficulty); // calls error permission, which checks how many times you may guess incorrectly
int numberOfFailures = 0;
int charAppearInWord = 0;
char guess = 'a';
#endregion

#region game
// creates the "hidden characters" depending on how many letters there are in the selected word
foreach (char letter in word){
    lettersInTheWord.Add(letter);
    solvedLettersInTheWord.Add('_');
}

while (wordSolved == false){ // starts the game loop. wordSolved is initiated as false
    /* First if statement here only runs once as to not show the "you may fail N times" each time
    the player gets a correct answer. Thus, "charAppearInWord" must be 0, as this value would only
    be 0 if the player hasn't guessed yet (e.g. before the player has started playing.) Once the
    played has failed, either charAppearInWord increases or the numberOfFailures increases, and
    thus it cannot play more than once. */
    if (numberOfFailures == 0 && charAppearInWord == 0){
        Console.WriteLine($"You may fail up to {numberOfPermittedFailures} time");
        if (numberOfPermittedFailures-numberOfFailures != 1){
            Console.Write("s");
        }
        Console.WriteLine("."); // above code copied to account for edge case where you instantiate with only 1 permitted failure
    }
    else if (numberOfFailures > 0){ // runs with a variable 
        Console.Write("You may fail " + (numberOfPermittedFailures-numberOfFailures) + " more time");
        if (numberOfPermittedFailures-numberOfFailures != 1){
            Console.Write("s");
        }
        Console.WriteLine("."); // see above
    }
    
    charAppearInWord = 0; // done to reset this value, not specified above since it would break the one-time check on the initial failure count

    for (int i = 0; i < lettersInTheWord.Count(); i++) // writes out the hidden and correctly guessed letters
    {
        if (i < solvedLettersInTheWord.Count()-1){ // -1 since count starts from 1 and i starts from 0
            Console.Write(solvedLettersInTheWord[i] + " ");
        }
        else{
            Console.WriteLine(solvedLettersInTheWord[i] + $" ({i+1})"); // fixing code jargon from above in text
        }
    }

    bool validAnswer;
    string playerRealGuess;
    string wordGuessed = "";
    do{ // do used to allow for an initial run without a useless variable
        Console.WriteLine("guess");
        playerRealGuess = Console.ReadLine();
        validAnswer = char.TryParse(playerRealGuess, out guess); // guess is changed from instantiation
        Console.WriteLine();
        if (wordGuessed != playerRealGuess){
            wordGuessed = "";
        }
        if (validAnswer == false && wordGuessed == "" && guess != 'a'){ // guess is instantiated as a, changed above
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Guessing a word suggests knowledge of the word. If you believe your answer is correct, re-type this word.");
            Console.WriteLine("If you are incorrect, you will immediately lose the game.");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("If this was an error, ignore this message and continue playing as normal.");
            Console.ForegroundColor = ConsoleColor.Gray;
            wordGuessed = playerRealGuess; // fix this
        } else if (validAnswer == false && wordGuessed != ""){
            if (wordGuessed.ToLower() == word.ToLower()){
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
#endregion

#region post-credits scene
if (wordSolved == true){
    Console.WriteLine("You Win! You get 1 000 000 000 dollars");
    Console.ReadLine();
} else{
    Console.WriteLine("Bevel in your disparity, loser");
    Console.ReadLine();
}
#endregion