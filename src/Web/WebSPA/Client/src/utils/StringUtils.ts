/**
 * Truncate a string to a certain length and append an ellipsis to the end.
 * @param str The string to truncate.
 * @param length The length to truncate the string to.
 * @param end The string to append to the end of the truncated string.
 * @param emptyStr The string to return if the input string is empty.
 * @param nullStr The string to return if the input string is null.
 * @param undefinedStr The string to return if the input string is undefined.
 * @returns The truncated string.
 */
export function truncateStringWith(
    str: string | null | undefined,
    length: number,
    end: string = "...",
    emptyStr: string = "空",
    nullStr: string = "null",
    undefinedStr: string = "undefined"
): string {
    if (str === null) return nullStr;
    else if (str === undefined) return undefinedStr;
    if (str.length === 0) return emptyStr;

    return str.length > length
        ? str.substring(0, length - end.length) + end
        : str;
}
