<template>
    <div class="quiz">
        <h1>Quiz</h1>
        <b-alert :show="warningText !== '' && warningText !== undefined" variant="danger" dismissible @dismissed="onDismiss()">{{warningText}}</b-alert>
        <div class="container-fluid">

            <div class="loading" v-if="initialLoad">
                <h2>Loading...</h2>
            </div>

            <div class="playing" v-if="!initialLoad && state == PlayState.Playing">
                Display questions here
            </div>

            <div class="playing" v-if="!initialLoad && state == PlayState.Won">
                You have won!
            </div>

            <div class="playing" v-if="!initialLoad && state == PlayState.Lost">
                You have lost!
            </div>

        </div>
    </div>
</template>

<script lang="ts">
    import { CurrentQuestion, nameof, PlayState } from '@/ResponseTypes';
    import GameApi from '@/services/GameApi';
    import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
    import { NavigationGuardNext, Route } from 'vue-router';

    @Component({
        components: {
            
        }
    })
    export default class Quiz extends Vue {

        private warningText: string = '';
        public state = PlayState.Playing;
        private currentQuestion = new CurrentQuestion();
        private initialLoad = true;

        constructor() {
            super();
        }

        async mounted() {
            this.handleState();
            await this.loadCurrentQuestion()
            this.initialLoad = false;
        }

        async loadCurrentQuestion() {
            this.currentQuestion = await GameApi.GetCurrentQuestion();;
        }

        async handleState() {
            this.state = await GameApi.CurrentState();
        }

        public onDismiss() {
            
        }

        @Watch(nameof<Quiz>('state'))
        public onStateChanged($new: PlayState, $old: PlayState) {

        }

        public static async beforeEnter(to: Route, 
                                  from: Route, 
                                  next: NavigationGuardNext<Vue>) {
            let state = await GameApi.CurrentState();
            console.log(state)
            console.log(state === PlayState.Menu)
            //if(state != PlayState.Menu) next('start')
            //else 
            next()
        }

    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
