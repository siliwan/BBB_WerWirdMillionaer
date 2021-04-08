interface Settings {
    protocol: string,
    hostname: string,
    port: number,
    apiEndpoint: string
}

declare global {
    var settings: Settings;
}

export default window.settings;