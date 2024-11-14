public static class WriteTimeNonAlloc
{
    public static void SecondsToTimeCharArray(this float timeInSeconds, char[] array)
    {
        int minutes = (int)(timeInSeconds / 60f);
        array[0] = (char)(48 + minutes * 0.1f);
        array[1] = (char)(48 + minutes % 10);
        array[2] = ':';

        int seconds = (int)(timeInSeconds - minutes * 60f);
        array[3] = (char)(48 + seconds * 0.1f);
        array[4] = (char)(48 + seconds % 10);
        array[5] = '.';

        int milliseconds = (int)((timeInSeconds % 1) * 1000);
        array[6] = (char)(48 + milliseconds * 0.01f);
        array[7] = (char)(48 + (milliseconds % 100) * 0.1f);
        array[8] = (char)(48 + milliseconds % 10);
    }
}
