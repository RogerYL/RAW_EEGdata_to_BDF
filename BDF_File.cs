namespace MPSA41CH.BDF_Library
{
    public static class BDF_File
    {
        // Generates a string with a specificied number of spaces
        private static string generateSpace(int length)
        {
            string outSpaces = "";
            for (int i = 0; i < length; i++)
                outSpaces += " ";

            return outSpaces;
        }

        // Uses the internal function generateSpaces() to add a string of spaces to the incoming header field
        public static string buildFile(string content, int length)
        {
            string outString = "";
            outString += content + generateSpace(length - content.Length);
            return outString;
        }
    }
}
