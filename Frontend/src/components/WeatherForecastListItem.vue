<template>
    <tr>
        <td>{{ forecast.date }}</td>
        <td>{{ forecast.temperatureC }}</td>
        <td>{{ forecast.temperatureF }}</td>
        <td>{{ forecast.summary }}</td>
    </tr>
</template>

<script lang="ts">
        import { Component, Prop, Vue } from 'vue-property-decorator';
        import { WeatherForecast } from '../WeatherForecast';

    @Component
    export default class WeatherForecastList extends Vue {
        @Prop() private forecast!: WeatherForecast
    }
</script>