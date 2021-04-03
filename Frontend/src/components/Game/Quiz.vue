<template>
    <div class="quiz">
        <h1>Quiz</h1>
        <b-alert :show="warningText !== '' && warningText !== undefined" variant="danger" dismissible @dismissed="onDismiss()">{{warningText}}</b-alert>
        <div class="container-fluid">

            <div class="loading" v-if="initialLoad">
                <h2>Loading...</h2>
            </div>

            <div class="playing container border rounded p-5" v-if="!initialLoad && state == 'Playing'">
                <div class="row">
                    <h2>{{ currentQuestion.question.questionText }}</h2>
                </div>
                <div class="row ">
                    <small>Answered correctly: {{ currentQuestion.percentCorrect }}% </small>
                </div>
                <div class="row mb-4">
                    <small>Category: {{ currentQuestion.question.category.name }}</small>
                </div>
                <div class="row justify-content-around mb-4">       
                    <div class="col mr-auto col-offset-2" v-for="answer in currentQuestion.question.answers.$values" :key="answer.id">
                        <b-button variant="primary" @click="submitAnswer($event, answer)">{{ answer.answerText }}</b-button>
                    </div>
                </div>
                <div class="row">
                    <b-icon icon="stopwatch" shift-v="-4"></b-icon>&nbsp;<countdown :CountdownUntil="currentQuestion.timeLeftUntil" @onCompleted="timeUp"></countdown>
                </div>
            </div>

            <div class="won container border rounded p-5" v-if="!initialLoad && state == 'Won'">
                <div class="row">
                    <p>You have won!</p>
                </div>
                <div class="row">
                    <b-button variant="primary" @click="reset">Reset</b-button>
                </div>
            </div>

            <div class="lost container border rounded p-5" v-if="!initialLoad && state == 'Lost'">
                <div class="row">
                    <p>You have lost!</p>
                </div>
                <div class="row">
                    <b-button variant="primary" @click="reset">Reset</b-button>
                </div>
            </div>

        </div>
    </div>
</template>

<script lang="ts">
    import { Answer, CurrentQuestion, nameof, PlayState } from '@/ResponseTypes';
    import GameApi from '@/services/GameApi';
    import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
    import { NavigationGuardNext, Route } from 'vue-router';
    import Countdown from '@/components/Game/Countdown.vue';

    @Component({
        components: {
            Countdown
        }
    })
    export default class Quiz extends Vue {

        private warningText: string = '';
        public state: PlayState = "Playing";
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
            try {
                this.currentQuestion = await GameApi.GetCurrentQuestion();
                console.log(this.currentQuestion)
            } catch (error) {
                this.reset();
            }
        }

        public reset() {
            this.$router.push('/quiz/start')
        }

        public async submitAnswer(evt: PointerEvent, answer: Answer) {

            if(answer !== undefined) {
                let result = await GameApi.SubmitAnswer(answer.id);

                if(result.object == 'TimeUp' || result.object == 'Invalid') {
                    //time is up?
                    this.warningText = result.message;
                } else if(result.object == 'Correct') {
                    //Load next question!
                    this.currentQuestion = await GameApi.GetCurrentQuestion();
                } else if(result.object == 'Lost') {
                    this.warningText = result.message;
                    this.state = result.object;
                } else {
                    this.state = result.object;
                }
            }
        }

        public timeUp() {
            alert("Time is up!")
            this.reset();
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
            if(state != "Playing") next('/quiz/start')
            else 
            next()
        }

    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
