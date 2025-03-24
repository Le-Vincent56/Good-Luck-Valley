namespace GoodLuckValley.Persistence
{
    public interface ISerializer
    {
        /// <summary>
        /// Serialize an object to a string
        /// </summary>
        string Serialize<T>(T obj) where T : Data;


        /// <summary>
        /// Deserialize a string into an object
        /// </summary>
        T Deserialize<T>(string json);
    }
}
