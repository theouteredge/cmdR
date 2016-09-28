namespace cmdR.IO
{
    public interface ICmdRConsole
    {
        string ReadLine();

        void Write(string line, params object[] parameters);
        void Write(string line);

        void WriteLine(string line, params object[] paramters);
        void WriteLine(string line);
    }
}
