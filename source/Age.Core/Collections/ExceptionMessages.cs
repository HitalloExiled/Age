namespace Age.Core.Collections;

internal static class ExceptionMessages
{
    internal const string ARG_ADDING_DUPLICATE_WITH_KEY                    = "An item with the same key has already been added. Key: {0}";
    internal const string ARG_ARRAY_LENGTHS_DIFFER                         = "Array lengths must be the same.";
    internal const string ARG_ARRAY_PLUS_OFF_TOO_SMALL                     = "Destination array is not long enough to copy all the items in the collection. Check array index and length.";
    internal const string ARG_BIT_SET_LENGTHS_DIFFER                       = "BitSet lengths must be the same.";
    internal const string ARG_KEY_NOT_FOUND_WITH_KEY                       = "Arg_KeyNotFoundWithKey";
    internal const string ARGUMENT_OUT_OF_RANGE_INDEX                      = "Index was out of range. Must be non-negative and less than the size of the collection.";
    internal const string ARGUMENT_OUT_OF_RANGE_MUST_BE_LESS_THAN_CAPACITY = "Value must be non-negative and less than or equal to collection capacity.";
    internal const string ARGUMENT_OUT_OF_RANGE_MUST_BE_NON_NEG_INT32      = "Value must be non-negative and less than or equal to Int32.MaxValue.";
    internal const string ARGUMENT_OUT_OF_RANGE_MUST_BE_NON_NEG_NUM        = "{0} must be non-negative.";
    internal const string ARGUMENT_OUT_OF_RANGE_MUST_BE_POSITIVE           = "{0} must be greater than zero.";
    internal const string ARGUMENT_OUT_OF_RANGE_NEED_NON_NEG_NUM           = "Non-negative number required.";
    internal const string CONCURRENT_OPERATION_ARE_NOT_SUPPORTED           = "Concurrent Operation are not supported.";
    internal const string INVALID_OPERATION_COLLECTION_FULL                = "Fixed size collection is full.";
    internal const string INVALID_OPERATION_EMPTY_HEAP                     = "Heap empty.";
    internal const string INVALID_OPERATION_EMPTY_LINKED_LIST              = "The LinkedList is empty.";
    internal const string INVALID_OPERATION_EMPTY_QUEUE                    = "Queue empty.";
    internal const string INVALID_OPERATION_EMPTY_STACK                    = "Stack empty.";
    internal const string INVALID_OPERATION_ENUM_FAILED_VERSION            = "Collection was modified; enumeration operation may not execute.";
    internal const string INVALID_OPERATION_ENUM_OP_CANT_HAPPEN            = "Enumeration has either not started or has already finished.";
}
