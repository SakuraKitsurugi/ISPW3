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
		BigInteger y = Encrypt(plainTextNumerical, publicKey, n);

		// Save everything into a file
		SaveToFile(y, publicKey);

		// Read everything from the file and display the information
		Console.Clear();
		Console.WriteLine($"Plaintext: {x}\nNumerical Plaintext: {plainTextNumerical}");
		var (readY, readE) = ReadFromFile();
		Console.WriteLine($"Read from file:\nEncrypted Plaintext: {readY}\nPublic Key: {readE}");
	}
	
	static bool IsPrime(int number) {
		for (int i = 2; i < number; i++) {
			if (number % i == 0 && i != number) return false;
		}
		return true;
	}
	
	static BigInteger TextToNumber(string text)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		return new BigInteger(bytes);
	}

	static int FindPublicKey(int phi)
	{
		// Find smallest possible i coprime (1 < i < φ(n))
		for (int i = 2; i < phi; i++)
			if (GreatestCommonDivisor(i, phi) == 1) return i;
		throw new Exception("Could not find a valid public key!");
	}

	static int GreatestCommonDivisor(int i, int phi) { 
		while (phi != 0)
		{
			int tempPhi = phi;
			phi = i % phi;
			i = tempPhi;
		}
		return i;
	}

	static BigInteger Encrypt(BigInteger text, BigInteger x, BigInteger y) {
		return BigInteger.ModPow(text, x, y);
	}

	static void SaveToFile(BigInteger y, int e)
	{
		File.WriteAllText("RSA.txt", $"{y}\n{e}");
	}

	static (BigInteger y, int publicKey) ReadFromFile()
	{
		string[] lines = File.ReadAllLines("RSA.txt");
		return (BigInteger.Parse(lines[0]), int.Parse(lines[1]));
	}
	
	static void LibraryDecryptionEncryption() {
		Console.WriteLine("Enter text to encrypt:");
		string plaintext = Console.ReadLine();

		// Generates RSA keys and proceeds to encrypt and decrypt.
		using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
		{
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