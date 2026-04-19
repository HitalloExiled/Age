namespace Age.Platforms.Linux;

public static class AsmGenericErrno
{
    public const uint EPERM   = 1;	/* Operation not permitted */
    public const uint ENOENT  = 2;	/* No such file or directory */
    public const uint ESRCH   = 3;	/* No such process */
    public const uint EINTR   = 4;	/* Interrupted system call */
    public const uint EIO     = 5;	/* I/O error */
    public const uint ENXIO   = 6;	/* No such device or address */
    public const uint E2BIG   = 7;	/* Argument list too long */
    public const uint ENOEXEC = 8;	/* Exec format error */
    public const uint EBADF   = 9;	/* Bad file number */
    public const uint ECHILD  = 10;	/* No child processes */
    public const uint EAGAIN  = 11;	/* Try again */
    public const uint ENOMEM  = 12;	/* Out of memory */
    public const uint EACCES  = 13;	/* Permission denied */
    public const uint EFAULT  = 14;	/* Bad address */
    public const uint ENOTBLK = 15;	/* Block device required */
    public const uint EBUSY   = 16;	/* Device or resource busy */
    public const uint EEXIST  = 17;	/* File exists */
    public const uint EXDEV   = 18;	/* Cross-device link */
    public const uint ENODEV  = 19;	/* No such device */
    public const uint ENOTDIR = 20;	/* Not a directory */
    public const uint EISDIR  = 21;	/* Is a directory */
    public const uint EINVAL  = 22;	/* Invalid argument */
    public const uint ENFILE  = 23;	/* File table overflow */
    public const uint EMFILE  = 24;	/* Too many open files */
    public const uint ENOTTY  = 25;	/* Not a typewriter */
    public const uint ETXTBSY = 26;	/* Text file busy */
    public const uint EFBIG   = 27;	/* File too large */
    public const uint ENOSPC  = 28;	/* No space left on device */
    public const uint ESPIPE  = 29;	/* Illegal seek */
    public const uint EROFS   = 30;	/* Read-only file system */
    public const uint EMLINK  = 31;	/* Too many links */
    public const uint EPIPE   = 32;	/* Broken pipe */
    public const uint EDOM    = 33;	/* Math argument out of domain of func */
    public const uint ERANGE  = 34;	/* Math result not representable */

    public const uint	EDEADLK      = 35;	/* Resource deadlock would occur */
    public const uint	ENAMETOOLONG = 36;	/* File name too long */
    public const uint	ENOLCK       = 37;	/* No record locks available */

    /*
    * This error code is special: arch syscall entry code will return
    * -ENOSYS if users try to call a syscall that doesn't exist.  To keep
    * failures of syscalls that really do exist distinguishable from
    * failures due to attempts to use a nonexistent syscall, syscall
    * implementations should refrain from returning -ENOSYS.
    */
    public const uint	ENOSYS =		38;	/* Invalid system call number */

    public const uint	ENOTEMPTY   = 39;	  /* Directory not empty */
    public const uint	ELOOP       = 40;	  /* Too many symbolic links encountered */
    public const uint	EWOULDBLOCK = EAGAIN; /* Operation would block */
    public const uint	ENOMSG      = 42;	  /* No message of desired type */
    public const uint	EIDRM       = 43;	  /* Identifier removed */
    public const uint	ECHRNG      = 44;	  /* Channel number out of range */
    public const uint	EL2NSYNC    = 45;	  /* Level 2 not synchronized */
    public const uint	EL3HLT      = 46;	  /* Level 3 halted */
    public const uint	EL3RST      = 47;	  /* Level 3 reset */
    public const uint	ELNRNG      = 48;	  /* Link number out of range */
    public const uint	EUNATCH     = 49;	  /* Protocol driver not attached */
    public const uint	ENOCSI      = 50;	  /* No CSI structure available */
    public const uint	EL2HLT      = 51;	  /* Level 2 halted */
    public const uint	EBADE       = 52;	  /* Invalid exchange */
    public const uint	EBADR       = 53;	  /* Invalid request descriptor */
    public const uint	EXFULL      = 54;	  /* Exchange full */
    public const uint	ENOANO      = 55;	  /* No anode */
    public const uint	EBADRQC     = 56;	  /* Invalid request code */
    public const uint	EBADSLT     = 57;	  /* Invalid slot */

    public const uint	EDEADLOCK =	EDEADLK;

    public const uint EBFONT          = 59;	/* Bad font file format */
    public const uint ENOSTR          = 60;	/* Device not a stream */
    public const uint ENODATA         = 61;	/* No data available */
    public const uint ETIME           = 62;	/* Timer expired */
    public const uint ENOSR           = 63;	/* Out of streams resources */
    public const uint ENONET          = 64;	/* Machine is not on the network */
    public const uint ENOPKG          = 65;	/* Package not installed */
    public const uint EREMOTE         = 66;	/* Object is remote */
    public const uint ENOLINK         = 67;	/* Link has been severed */
    public const uint EADV            = 68;	/* Advertise error */
    public const uint ESRMNT          = 69;	/* Srmount error */
    public const uint ECOMM           = 70;	/* Communication error on send */
    public const uint EPROTO          = 71;	/* Protocol error */
    public const uint EMULTIHOP       = 72;	/* Multihop attempted */
    public const uint EDOTDOT         = 73;	/* RFS specific error */
    public const uint EBADMSG         = 74;	/* Not a data message */
    public const uint EOVERFLOW       = 75;	/* Value too large for defined data type */
    public const uint ENOTUNIQ        = 76;	/* Name not unique on network */
    public const uint EBADFD          = 77;	/* File descriptor in bad state */
    public const uint EREMCHG         = 78;	/* Remote address changed */
    public const uint ELIBACC         = 79;	/* Can not access a needed shared library */
    public const uint ELIBBAD         = 80;	/* Accessing a corrupted shared library */
    public const uint ELIBSCN         = 81;	/* .lib section in a.out corrupted */
    public const uint ELIBMAX         = 82;	/* Attempting to link in too many shared libraries */
    public const uint ELIBEXEC        = 83;	/* Cannot exec a shared library directly */
    public const uint EILSEQ          = 84;	/* Illegal byte sequence */
    public const uint ERESTART        = 85;	/* Interrupted system call should be restarted */
    public const uint ESTRPIPE        = 86;	/* Streams pipe error */
    public const uint EUSERS          = 87;	/* Too many users */
    public const uint ENOTSOCK        = 88;	/* Socket operation on non-socket */
    public const uint EDESTADDRREQ    = 89;	/* Destination address required */
    public const uint EMSGSIZE        = 90;	/* Message too long */
    public const uint EPROTOTYPE      = 91;	/* Protocol wrong type for socket */
    public const uint ENOPROTOOPT     = 92;	/* Protocol not available */
    public const uint EPROTONOSUPPORT = 93;	/* Protocol not supported */
    public const uint ESOCKTNOSUPPORT = 94;	/* Socket type not supported */
    public const uint EOPNOTSUPP      = 95;	/* Operation not supported on transport endpoint */
    public const uint EPFNOSUPPORT    = 96;	/* Protocol family not supported */
    public const uint EAFNOSUPPORT    = 97;	/* Address family not supported by protocol */
    public const uint EADDRINUSE      = 98;	/* Address already in use */
    public const uint EADDRNOTAVAIL   = 99;	/* Cannot assign requested address */
    public const uint ENETDOWN        = 100;	/* Network is down */
    public const uint ENETUNREACH     = 101;	/* Network is unreachable */
    public const uint ENETRESET       = 102;	/* Network dropped connection because of reset */
    public const uint ECONNABORTED    = 103;	/* Software caused connection abort */
    public const uint ECONNRESET      = 104;	/* Connection reset by peer */
    public const uint ENOBUFS         = 105;	/* No buffer space available */
    public const uint EISCONN         = 106;	/* Transport endpoint is already connected */
    public const uint ENOTCONN        = 107;	/* Transport endpoint is not connected */
    public const uint ESHUTDOWN       = 108;	/* Cannot send after transport endpoint shutdown */
    public const uint ETOOMANYREFS    = 109;	/* Too many references: cannot splice */
    public const uint ETIMEDOUT       = 110;	/* Connection timed out */
    public const uint ECONNREFUSED    = 111;	/* Connection refused */
    public const uint EHOSTDOWN       = 112;	/* Host is down */
    public const uint EHOSTUNREACH    = 113;	/* No route to host */
    public const uint EALREADY        = 114;	/* Operation already in progress */
    public const uint EINPROGRESS     = 115;	/* Operation now in progress */
    public const uint ESTALE          = 116;	/* Stale file handle */
    public const uint EUCLEAN         = 117;	/* Structure needs cleaning */
    public const uint ENOTNAM         = 118;	/* Not a XENIX named type file */
    public const uint ENAVAIL         = 119;	/* No XENIX semaphores available */
    public const uint EISNAM          = 120;	/* Is a named type file */
    public const uint EREMOTEIO       = 121;	/* Remote I/O error */
    public const uint EDQUOT          = 122;	/* Quota exceeded */
    public const uint ENOMEDIUM       = 123;	/* No medium found */
    public const uint EMEDIUMTYPE     = 124;	/* Wrong medium type */
    public const uint ECANCELED       = 125;	/* Operation Canceled */
    public const uint ENOKEY          = 126;	/* Required key not available */
    public const uint EKEYEXPIRED     = 127;	/* Key has expired */
    public const uint EKEYREVOKED     = 128;	/* Key has been revoked */
    public const uint EKEYREJECTED    = 129;	/* Key was rejected by service */

    /* for robust mutexes */
    public const uint EOWNERDEAD      = 130;	/* Owner died */
    public const uint ENOTRECOVERABLE = 131;	/* State not recoverable */
    public const uint ERFKILL         = 132;	/* Operation not possible due to RF-kill */
    public const uint EHWPOISON       = 133;	/* Memory page has hardware error */
}
