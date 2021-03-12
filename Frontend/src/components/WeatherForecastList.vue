<template>
    <div class="weatherforecastlist">
        <span v-if="error">
            Something went wrong: {{ error }}
        </span>
        <table>
            <tr>
                <th>Datum</th>
                <th>C°</th>
                <th>F°</th>
                <th>Zusammenfassung</th>
            </tr>
            <WeatherForecastListItem v-for="dataRow of dataRows" 
                :forecast="dataRow"
                v-bind:key="dataRow.date" />
        </table>
    </div>
</template>

<script lang="ts">
        import { Component, Vue } from 'vue-property-decorator';
        import { WeatherForecast } from "../WeatherForecast";
        import WeatherForecastListItem from "./WeatherForecastListItem.vue";
        import axios from "axios";

    @Component({
      components: { WeatherForecastListItem }  
    })
    export default class WeatherForecastList extends Vue {
        private dataRows: WeatherForecast[] = [];
        private error: string = '';

        private _handle: number;

        constructor() {
            super()
            console.log('WheaterForecastList created')

            this.getData();
            this._handle = setInterval(this.getData, 5 * 1000);
        }

        async getData() {
           try {
                var res = await axios.get<WeatherForecast[]>(`/api/WeatherForecast`);
            
                this.dataRows = res.data;
                
                console.log(res.status)

                if(res.status >= 400) {
                    this.error = res.statusText;
                } else {
                    this.error = '';
                }
           } catch (error) {
               this.error = error;
           }
        }
    
    }
</script>

<!--Add "scoped" attribute to limit CSS to this component only-- >
<style scoped>
</style>
