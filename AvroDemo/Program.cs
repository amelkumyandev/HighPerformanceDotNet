using Avro.IO;
using Avro.Specific;
using AvroDemo.Models;

namespace AvroDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an instance of User
            var user = new User
            {
                Name = "John Doe",
                FavoriteNumber = 7,
                FavoriteColor = "Green"
            };

            // Serialize the user object to a byte array
            byte[] serializedData;
            using (var memoryStream = new MemoryStream())
            {
                var encoder = new BinaryEncoder(memoryStream);
                var writer = new SpecificDatumWriter<User>(user.Schema);
                writer.Write(user, encoder);
                serializedData = memoryStream.ToArray();
            }

            Console.WriteLine("User serialized successfully.");

            // Deserialize the byte array back to a User object
            User deserializedUser;
            using (var memoryStream = new MemoryStream(serializedData))
            {
                var decoder = new BinaryDecoder(memoryStream);
                var reader = new SpecificDatumReader<User>(user.Schema, user.Schema);
                deserializedUser = reader.Read(null, decoder);
            }
            Console.WriteLine($"Size of serialized data: {serializedData.Length} bytes");

            Console.WriteLine("User deserialized successfully.");
            Console.WriteLine($"Name: {deserializedUser.Name}");
            Console.WriteLine($"Favorite Number: {deserializedUser.FavoriteNumber}");
            Console.WriteLine($"Favorite Color: {deserializedUser.FavoriteColor}");
        }
    }
}
