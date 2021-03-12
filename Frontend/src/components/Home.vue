<template>
    <div class="home">
        <h1>{{ msg }}</h1>
        <p>Welcome to your new single-page application, built with <a href="https://vuejs.org" target="_blank">Vue.js</a> and <a href="http://www.typescriptlang.org/" target="_blank">TypeScript</a>.</p>
        <WeatherForecastList/>
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';
    import WeatherForecastList from "./WeatherForecastList.vue";

    @Component({
        components: { WeatherForecastList }
    })
    export default class Home extends Vue {
        @Prop() private msg!: string;

        constructor() {
            super()
            console.log('Home created')
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
