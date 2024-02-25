namespace VariablenUndDatentypen;

internal class Program
{
    static void Main(string[] args)
    {
        // 1. Grundlegende Datentypen
        int zahl = 42; // Deklaration und Initialisierung einer Ganzzahl-Variable
        Console.WriteLine("Die Zahl ist: " + zahl);

        float kommaZahl = 3.14f; // Deklaration und Initialisierung einer Fließkommazahl-Variable
        Console.WriteLine("Die Fließkommazahl ist: " + kommaZahl);

        char zeichen = 'A'; // Deklaration und Initialisierung einer Zeichen-Variable
        Console.WriteLine("Das Zeichen ist: " + zeichen);

        bool wahrheitswert = true; // Deklaration und Initialisierung einer Booleschen Variable
        Console.WriteLine("Der Wahrheitswert ist: " + wahrheitswert);

        // 3. Zeichenketten (Strings)
        string text = "CodeCommander ist Klasse!"; // Deklaration und Initialisierung einer Zeichenketten-Variable
        Console.WriteLine("Der Text lautet: " + text);

        // 4. Verwendung von Konstanten
        const double pi = 3.14159265359; // Deklaration und Initialisierung einer Konstanten. Konstanten können zur Laufzeit nicht mehr verändert werden.
        Console.WriteLine("Der Wert von Pi ist: " + pi);

        // 5. Typumwandlung (Casting)
        int ganzeZahl = 10;

        /*
         * Eine implizite Konvertierung kann für integrierte numerische Typen durchgeführt werden, 
         * wenn der zu speichernde Wert in die Variable passt, ohne abgeschnitten oder abgerundet zu werden.
         */
        double gleitkommazahl = ganzeZahl; // Implizite Typumwandlung
        Console.WriteLine("(1) Die Gleitkommazahl ist: " + gleitkommazahl);

        /*
        * Wenn allerdings eine Konvertierung nicht ohne möglichen Informationsverlust durchgeführt werden kann, 
        * fordert der Compiler eine explizite Konvertierung; diese wird als Umwandlung bezeichnet.
        */
        gleitkommazahl = double.Parse(ganzeZahl.ToString());
        Console.WriteLine("(2) Die Gleitkommazahl ist: " + gleitkommazahl);

        gleitkommazahl = double.Parse("10");
        Console.WriteLine("(3) Die Gleitkommazahl ist: " + gleitkommazahl);

        gleitkommazahl = (double)ganzeZahl; // Explizite Typumwandlung
        Console.WriteLine("(4) Die Gleitkommazahl ist: " + gleitkommazahl);

        // 6. Praktische Beispiele
        string vorname = "Max";
        string nachname = "Mustermann";
        string vollerName = vorname + " " + nachname; // String-Konkatenation
        Console.WriteLine("Der vollständige Name ist: " + vollerName);

        // 7. Dynamisches setzen von Datentypen
        var halbDynamischerDatentyp = "test"; //var übernimmt den Datentyp, der initial durch den Wert der eingetragen wird festgelegt ist.
        //Nicht möglich: halbDynamischerDatentyp = 15;


        dynamic dynamischerDatentyp = "Test"; //Nicht empfohlen: Reduziert die Les- und Wartbarkeit und kann zu fehlern führen!
        dynamischerDatentyp = 15;

        Console.ReadLine();
    }
}
