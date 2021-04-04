<template>
    <div class="quiz">
        <div class="container">
            <b-row>
                <b-alert :show="warningText !== '' && warningText !== undefined" variant="danger" dismissible @dismissed="onDismiss()">{{warningText}}</b-alert>
            </b-row>

            <b-row class="loading" v-if="initialLoad">
                <p class="h2">Loading...</p>
            </b-row>

            <b-container class="playing border rounded p-5" v-if="!initialLoad && state == 'Playing'">
                <b-row>
                    <p class="h2">
                        {{ currentQuestion.question.questionText }}
                    </p>
                </b-row>
                <b-row>
                    <p class="small">
                        Answered correctly: {{ currentQuestion.percentCorrect }}%
                    </p>
                </b-row>
                <b-row class="mb-4">
                    <p class="small">
                        Category: {{ currentQuestion.question.category.name }}
                    </p>
                </b-row>
                <b-row class="justify-content-around mb-4">
                    <b-col class="mr-auto col-offset-2 mb-2" cols="6" v-for="answer in currentQuestion.question.answers.$values" :key="answer.id">
                        <b-button class="wwm-btn" variant="primary" @click="submitAnswer($event, answer)">{{ answer.answerText }}</b-button>
                    </b-col>
                </b-row>
                <b-row>
                    <b-icon icon="stopwatch" shift-v="-4"></b-icon>&nbsp;<countdown :CountdownUntil="currentQuestion.timeLeftUntil" @onCompleted="timeUp"></countdown>
                </b-row>
                <b-row>
                    <b-button variant="primary" @click="cashIn">Cash In</b-button>
                </b-row>
            </b-container>

            <b-container class="won border rounded p-5" v-if="!initialLoad && state == 'Won'">
                <b-row>
                    <p>You have won!</p>
                </b-row>
                <b-form-row class="mb-2">
                    <b-col>
                        <b-input type="text" v-model="highscoreName">Your name</b-input>
                    </b-col>
                    <b-col>
                        <b-button variant="primary" @click="submitHighscore">Enter highscore</b-button>
                    </b-col>
                </b-form-row>
                <b-row>
                    <b-col>
                        <b-button variant="primary" @click="reset">Reset</b-button>
                    </b-col>
                </b-row>
            </b-container>

            <b-container class="border rounded p-5" v-if="!initialLoad && state == 'Lost'">
                <b-row>
                    <p>You have lost!</p>
                </b-row>
                <b-row>
                    <b-button variant="primary" @click="reset">Reset</b-button>
                </b-row>
            </b-container>

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
        private highscoreName = '';

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

        async cashIn() {
            await GameApi.CashIn();
            this.handleState();
        }

        async submitHighscore() {

            if(this.highscoreName === undefined || this.highscoreName.trim() === '') {
                this.warningText = "You must provide a name for it to be seen in the highscores!"
            } else {
                await GameApi.SubmitHighscore(this.highscoreName);
            }

        }

        timeUp() {
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
<style lang="sass" scoped>
    .wwm-btn {
        width: 100%;

        :after {
            content: "";
            display: inline-block;
            width: 0;
            height: 0;
            border-top: 20px solid rgb(0, 0, 0);
            border-bottom: 20px solid rgb(0, 0, 0);
            border-left: 20px solid red;
        }
    }
</style>
