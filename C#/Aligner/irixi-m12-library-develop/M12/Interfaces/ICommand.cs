namespace M12.Interfaces
{
    public interface ICommand
    {
        /// <summary>
        /// Convert the command to byte array.
        /// </summary>
        /// <returns></returns>
        byte[] ToArray();

    }
}