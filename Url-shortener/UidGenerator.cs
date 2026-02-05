public static class UidGenerator
{
    //  Generating a unique id based on the current time and a random number
    //  Simplest realization of Snowflake algorithm
    //  As alternative to Snowflake algorithm, you can use UUIDs, but they are longer
    public static ulong GenerateUid()      //  Using ulong to avoid negative values
    {
        DateTime time_now = DateTime.Now;
        long binary = time_now.Ticks;
        binary >>= 16;  // Generate 16 zeroes to the left
        Random random = new Random();
        long random_value = random.Next(0, 65536);
        long result = binary | (random_value << 48); // Shifting random value to not conflict with time
        return (ulong)result;
    }
}