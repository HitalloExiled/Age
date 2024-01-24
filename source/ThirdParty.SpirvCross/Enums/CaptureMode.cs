namespace ThirdParty.SpirvCross.Enums;

public enum CaptureMode
{
    /// <summary>
    /// The Parsed IR payload will be copied, and the handle can be reused to create other compiler instances.
    /// </summary>
	Copy = 0,


	/// <summary>
    /// The payload will now be owned by the compiler.
    /// parsed_ir should now be considered a dead blob and must not be used further.
    /// This is optimal for performance and should be the go-to option.
    /// </summary>
	TakeOwnership = 1,

	IntMax = 0x7fffffff
}
