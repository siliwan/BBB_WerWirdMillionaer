<template>
    <div id="app">
        <b-button variant="danger" @click="onClear">Clear storage</b-button>
        <router-view></router-view>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import Home from './components/Home.vue';

    @Component({
        components: {
            Home
        }
    })
    export default class App extends Vue { 

        onClear(evt: PointerEvent) {
            localStorage.removeItem('x-quiz-session-id');
            try {
                this.$router.push('/')
            } catch (error) {
                
            }
        }
    }
</script>

<style>
</style>
