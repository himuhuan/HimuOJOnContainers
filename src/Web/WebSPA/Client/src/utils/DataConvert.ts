
/**
 * Convert time span to milliseconds
 * @param timeSpan time span in format hh:mm:ss, only support 24 hours format 
 * @returns time span in milliseconds
 * @example "01:00:00" => 3600000
 */
export function convertTimeSpanToMilliseconds(timeSpan: string): number {
    const timeSpanArray = timeSpan.split(":");
    const hour = parseInt(timeSpanArray[0]);
    const minute = parseInt(timeSpanArray[1]);
    const second = parseInt(timeSpanArray[2]);
    return hour * 3600 * 1000 + minute * 60 * 1000 + second * 1000;
}

/**
 * Convert milliseconds to time span
 * @param milliseconds time span in milliseconds
 * @returns time span in format hh:mm:ss
 * @example 3600000 => "01:00:00"
 */
export function convertMillisecondsToTimeSpan(milliseconds: number): string
{
    const hour = Math.floor(milliseconds / 3600000);
    const minute = Math.floor((milliseconds % 3600000) / 60000);
    const second = Math.floor((milliseconds % 60000) / 1000);
    return `${hour.toString().padStart(2, "0")}:${minute.toString().padStart(
        2,
        "0"
    )}:${second.toString().padStart(2, "0")}`;
}