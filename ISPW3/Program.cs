using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace ISPW3;

class Program {
	static void Main(string[] args) {
		//RSA Decryption and Encryption example using Cryptography library.
		//LibraryDecryptionEncryption();

		int p; // Primary number p
		int q; // Primary number q
		string x; // Plaintext string
		BigInteger plainTextNumerical; // Plaintext numerical value

		// Primary number input with limitations of <=1000
		while (true) {
			Console.Clear();
			Console.WriteLine("Enter prime p (<=1000):");
			p = int.Parse(Console.ReadLine());

			Console.WriteLine("Enter prime q (<=1000):");
			q = int.Parse(Console.ReadLine());

			if (!IsPrime(p) || !IsPrime(q) || p > 1000 || q > 1000 || p == q) {
				Console.WriteLine("Incorrect primes! Must be less than 1000, a prime number and not the same.");
				Console.ReadLine();
			}
			else {
				break;
			}
		}

		// Calculate n and φ(n) (φ(n) is calculated using Euler’s Totient Function)
		int n = p * q;
		int phi = (p - 1) * (q - 1);

		// Plaintext input, conversion into numberical value and x < n check
		while (true) {
			Console.Clear();
			Console.WriteLine("Enter plaintext:");
			x = Console.ReadLine();
			plainTextNumerical = TextToNumber(x);
			if (plainTextNumerical >= n) {
				Console.WriteLine("Plaintext numerical expression is too big!");
			}
			else {
				break;
			}
		}

		// Find public key using φ(n)
		int publicKey = FindPublicKey(phi);

		// Encrypt x using ModPow
		BigInteger encrypted = Encrypt(plainTextNumerical, publicKey, n);
		
		// Save everything into a file
		SaveToFile(encrypted, publicKey);

		// Find public key using inverse formula
		int privateKey = FindPrivateKey(publicKey, phi);

		// Decrypt y using ModPow
		BigInteger decrypted = Decrypt(encrypted, privateKey, n);

		// Brute force p and q - PLACEHOLDER DUE TO LACK OF KNOWLEDGE
		//(int pBrute, int qBrute) = BruteForcePrimes();

		// Read everything from the file and display the information
		Console.Clear();
		Console.WriteLine($"Plaintext: {x}\nNumerical Plaintext: {plainTextNumerical}");
		var (readText, readPuKey) = ReadFromFile();
		Console.WriteLine(
			$"Read from file:\nEncrypted Plaintext: {readText} & Public Key: {readPuKey}\nPrivate Key: {privateKey}");
		Console.WriteLine($"Decrypted: {decrypted}");
		//Console.WriteLine($"Brute forced p and q: {pBrute} & {qBrute}");
	}

	// Checks if the passed number is a prime or not
	static bool IsPrime(int number) {
		for (int i = 2; i < number; i++) {
			if (number % i == 0 && i != number) return false;
		}

		return true;
	}

	// Converts text input into numerical value, but only works on very short words
	static BigInteger TextToNumber(string text) {
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		return new BigInteger(bytes);
	}

	// Finds public key e by finding the smallest possible comprime (1 < i < φ(n))
	static int FindPublicKey(int phi) {
		for (int i = 2; i < phi; i++)
			if (GCD(i, phi) == 1)
				return i;
		throw new Exception("Could not find a valid public key!");
	}

	// Finds private key
	static int FindPrivateKey(int publicKey, int phi) {
		return ModInverse(publicKey, phi);
	}

	// Greatest Common Divisor
	static int GCD(int i, int phi) {
		while (phi != 0) {
			int tempPhi = phi;
			phi = i % phi;
			i = tempPhi;
		}

		return i;
	}

	// Finds private key D using inverse mod. I have no clue how to mathematically calculate this and have to completely rely on the code.
	static int ModInverse(int puKey, int phi) {
		int phiCopy = phi;
		int y = 0, x = 1;

		if (phi == 1)
			return 0;

		while (puKey > 1) {
			// q is quotient
			int q = puKey / phi;
			int t = phi;

			// phi == remainder,
			phi = puKey % phi;
			puKey = t;
			t = y;

			// Update y and x
			y = x - q * y;
			x = t;
		}

		// Make x positive
		if (x < 0)
			x += phiCopy;

		return x;
	}

	// Using brute force method finds p and q from encrypted text and public key
	static (int, int) BruteForcePrimes(int phi) {
		// Precompiles all primary numbers with the int upper limit of 1000
		List<int> primeList = new List<int>();
		for (int i = 2; i <= 1000; i++) {
			if (IsPrime(i)) {
				primeList.Add(i);
			}
		}
		
		
			
		return (0, 0);
	}

	// Encryption method
	static BigInteger Encrypt(BigInteger text, BigInteger publicKey, BigInteger n) {
		return BigInteger.ModPow(text, publicKey, n);
	}

	// Decryption method
	static BigInteger Decrypt(BigInteger text, BigInteger privateKey, BigInteger n) {
		return BigInteger.ModPow(text, privateKey, n);
	}

	// Saves encrypted text and public key into a file
	static void SaveToFile(BigInteger encryptedText, int publicKey) {
		File.WriteAllText("RSA.txt", $"{encryptedText}\n{publicKey}");
	}

	// Reads information from the RSA.txt file
	static (BigInteger encryptedText, int publicKey) ReadFromFile() {
		string[] lines = File.ReadAllLines("RSA.txt");
		return (BigInteger.Parse(lines[0]), int.Parse(lines[1]));
	}

	// Simple showcase of RSA encryption and decryption using the cryptography library
	static void LibraryDecryptionEncryption() {
		Console.WriteLine("Enter text to encrypt:");
		string plaintext = Console.ReadLine();

		// Generates RSA keys and proceeds to encrypt and decrypt.
		using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048)) {
			// Encryption
			byte[] encrypted = rsa.Encrypt(Encoding.UTF8.GetBytes(plaintext), false);
			Console.WriteLine($"Encrypted: {Convert.ToBase64String(encrypted)}");
			Console.WriteLine($"Key: {rsa.ToXmlString(false)}");

			// Decryption
			byte[] decrypted = rsa.Decrypt(encrypted, false);
			Console.WriteLine($"Decrypted: {Encoding.UTF8.GetString(decrypted)}");
		}
	}
}