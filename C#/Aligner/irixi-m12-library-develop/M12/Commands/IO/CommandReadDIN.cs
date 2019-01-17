namespace M12.Commands.IO
{
    public class CommandReadDIN  : CommandBase
    {
        public CommandReadDIN() : base()
        {

        }
        
        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_READ_DIN;
            }
        }

    }
}
