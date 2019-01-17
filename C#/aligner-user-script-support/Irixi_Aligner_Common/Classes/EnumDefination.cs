namespace Irixi_Aligner_Common
{
    public enum MoveMode
    {
        ABS,
        REL
    }

    public enum SystemState
    {
        USER_SCRIPT_RUN,
        USER_SCRIPT_PAUSE,
        IDLE,
        BUSY
    }

    public enum MotionControllerType
    {
        LUMINOS_P6A,
        IRIXI_EE0017,
        IRIXI_M12,
    }
}
