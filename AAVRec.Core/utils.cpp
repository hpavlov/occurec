#include "StdAfx.h"
#include "utils.h"


void WriteString(FILE* pFile, const char* str)
{
	unsigned char len;
	len = strlen(str);
	
	fwrite(&len, 1, 1, pFile);
	fputs(str, pFile);
}

void DbgPrintBytes(unsigned char *bytes, int maxLen)
{
	for(int i = 0; i < maxLen; i++)
	{
		printf("%x", bytes[i]);
		if (i % 10 == 0) printf("\r\n");
	}
}


unsigned int crctab[256];

void crc32_init(void)
{
    int i,j;

    unsigned int crc;

    for (i = 0; i < 256; i++)
    {
        crc = i << 24;
        for (j = 0; j < 8; j++)
        {
            if (crc & 0x80000000)
                crc = (crc << 1) ^ 0x04c11db7;
            else
                crc = crc << 1;
        }
        crctab[i] = crc;
    }
}

unsigned int compute_crc32(unsigned char *data, int len)
{
    unsigned int        result;
    int                 i;
    unsigned char       octet;
    
    result = *data++ << 24;
    result |= *data++ << 16;
    result |= *data++ << 8;
    result |= *data++;
    result = ~ result;
    len -=4;
    
    for (i=0; i<len; i++)
    {
        result = (result << 8 | *data++) ^ crctab[result >> 24];
    }
    
    return ~result;
}

// Fri, 01 Jan 2010 00:00:00 GMT (http://www.epochconverter.com/)
#define ADV_EPOCH_ZERO 1262304000

time_t TIME_ADV_ZERO;

void InitAavTicksConversion()
{
	TIME_ADV_ZERO = static_cast<time_t>(ADV_EPOCH_ZERO);
}

long long DateTimeToAavTicks(int year, int month, int day, int hour, int minute, int sec, int tenthMs)
{
	// NOTE: Sometimes we may get a blank date as 0/0/2000 00:00:00 and the attempt to convert it will crash
	
	//printf("DateTimeToAavTicks: %d-%d-%d %d:%d:%d\n", year, month, day, hour, minute, sec);
	if (year > 0 && month  > 0 && day > 0)
	{
		//ptime t(boost::gregorian::date(year,month,day));
		//ptime start(boost::gregorian::date(2010,1,1)); 
		//time_duration dur = t - start; 
		//time_t epoch = dur.total_seconds();

		//double diff = static_cast<long long>(epoch) + 3600 *  hour + 60 *  minute + sec;
		
		//long long advTicks = 10000 * (long long)(diff) + tenthMs;
		
		//return advTicks;		
	}
	else
		return 0;
}

void AvdTicksConversionTest()
{
    /*
	long long advTicks = DateTimeToAavTicks(2013, 1, 23, 12, 23, 56, 8716);
	
	int year;
	int month;
	int day;
	int hour;
	int minute;
	int sec;
	int ms;
	
	AavTicksToDateTime(advTicks, &year, &month, &day, &hour, &minute, &sec, &ms);
	
	printf("2013-1-23 12:23:56.8716\n");
	printf("%d-%d-%d %d:%d:%d.%d\n", year, month, day, hour, minute, sec, ms);
	*/
}

//long long UnixHiResTimeToAavTicks(struct timeval unixHiResTime)
//{
//	double seconds = unixHiResTime.tv_sec - ADV_EPOCH_ZERO;
//	double fraction = unixHiResTime.tv_usec;
//    double diff = fraction/1000000.0;
//
//	diff+= seconds;
//	
//	return (long long)(10000 * diff);
//}
//
//std::time_t convert(const boost::posix_time::ptime& t)
//{
//   static const boost::posix_time::ptime epoch(
//         boost::gregorian::date(1970,1,1) );
//   const boost::posix_time::time_duration::sec_type x(
//         (t - epoch).total_seconds() );
//   return x;
//}

void AavTicksToDateTime(long long ticks, int *year, int *month, int *day, int *hour, int *minute, int *sec, int *ms)
{
	// the miliseconds since 1 Jan 2000, 00:00:00.000 (negative vaslues are before 2000)
	
	*ms = (ticks % 10000);
	long long durationInSec = ticks / 10000;
	
	*sec = durationInSec % 60;
	long long durationInMin = durationInSec / 60;
	
	*minute = durationInMin % 60;
	long long durationInHours = durationInMin / 60;
	
	*hour = durationInHours % 24;
	long long durationInDays = durationInHours / 24;
	
	//ptime start(boost::gregorian::date(2010,1,1)); 
	//
	//time_t durationInEpoch = static_cast<time_t>(durationInDays * 24 * 3600 + convert(start));

	//boost::posix_time::ptime dt = boost::posix_time::from_time_t(durationInEpoch);
	//
	//boost::gregorian::date d = dt.date();

	//*year = d.year();
	//*month = d.month();
	//*day = d.day();
};