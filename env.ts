import { Option } from "@nozzlegear/railway";

/**
 * Gets an environment variable from process.env.
 */
export const env = (key: string) => {
    const value = process.env[key];

    return value ? Option.ofSome<string>(value) : Option.ofNone();
};

/**
 * Gets an environment variable from process.env, but throws an error if it is null or undefined.
 */
export const envRequired = (key: string) => {
    const value = env(key);

    if (value.isNone()) {
        throw new Error(`Required env variable ${key} was null or undefined.`);
    }

    return value.get();
};
