<template>
    <div class="quiz">
        <div class="container">
            <b-row>
                <b-col>
                    <b-alert :show="warningText !== '' && warningText !== undefined" variant="danger" dismissible @dismissed="onDismiss()">{{warningText}}</b-alert>
                </b-col>
            </b-row>

            <b-row class="loading" v-if="initialLoad">
                <b-col>
                    <p class="h2">Loading...</p>
                </b-col>
            </b-row>

            <b-container class="playing border rounded pt-2 pb-2 pl-5 pr-5" v-if="!initialLoad && state == 'Playing'">
                <b-row>
                    <b-col cols="3">
                        <b-button-group>
                            <b-button variant="primary" @click="cashIn">Cash In</b-button>
                            <b-button variant="primary" :disabled="!hasJoker" @click="useJoker">Use Joker</b-button>
                        </b-button-group>
                    </b-col>
                </b-row>
                <b-row class="mb-1">
                    <b-col>
                        <b-img width="200%" height="200%" src="@/assets/images/WWM.png" fluid center rounded="circle" alt="Who wants to be a millionaire" />
                    </b-col>
                </b-row>
                <b-row class="mb-2">
                    <b-col class="wwm p-2">
                        <p class="h2 text-center">
                                {{ currentQuestion.question.questionText }}
                        </p>
                    </b-col>
                </b-row>
                <b-row class="mb-4">
                    <b-col class="mr-auto col-offset-2 mb-2" cols="6" v-for="answer in currentQuestion.question.answers.$values" :key="answer.id">
                        <b-button class="wwm btn text-center text-wrap" @click="submitAnswer($event, answer)">{{ answer.answerText }}</b-button>
                    </b-col>
                </b-row>
                <hr />
                <b-row>
                    <b-col>
                        <countdown class="text-center" :CountdownUntil="currentQuestion.timeLeftUntil" @onCompleted="timeUp"><b-icon class="mr-1" icon="stopwatch" shift-v="0" /></countdown>
                    </b-col>
                    <b-col>
                        <p class="small text-center">
                            Answered correctly: {{ currentQuestion.percentCorrect }}%
                        </p>
                    </b-col>
                    <b-col>
                        <p class="small text-center">
                            Category: {{ currentQuestion.question.category.name }}
                        </p>
                    </b-col>
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
        private hasJoker: boolean = false;

        constructor() {
            super();
        }

        async mounted() {
            this.handleState();
            await this.loadCurrentQuestion()
            this.hasJoker = await GameApi.HasJoker();
            this.initialLoad = false;
        }

        async loadCurrentQuestion() {
            try {
                this.currentQuestion = await GameApi.GetCurrentQuestion();
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

        async useJoker() {
            if(this.hasJoker) {
                await GameApi.UseJoker();
                this.hasJoker = false;
                this.loadCurrentQuestion();      
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
                try {
                    await GameApi.SubmitHighscore(this.highscoreName);
                    this.reset();
                } catch (error) {
                    this.$bvToast.toast("Could not submit highscore!", {
                        title: 'Error',
                        variant: 'danger',
                        solid: true
                    });
                }
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
            if(state != "Playing") next('/quiz/start')
            else 
            next()
        }

    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="scss" scoped>
    @import '~bootstrap/scss/bootstrap.scss';
    $wwm-col: rgb(0, 3, 190);
    $wwm-col-hover: darken($wwm-col, 10);

    .wwm {
        color: #fff;

        &.btn, >.btn {
            padding-left: 30%;
            padding-right: 30%;
            width: 100%;
            background-color: $wwm-col;
            border: none;
            white-space: normal;
            &:hover {
                background-color: $wwm-col-hover;
            }
        }

        &p, >p {
            width: 30%; 
            margin: 0 auto;
        }

        background-color: $wwm-col;

        -webkit-clip-path: polygon(25% 0%, 75% 0%, 80% 50%, 75% 100%, 25% 100%, 20% 50%);
        clip-path: polygon(25% 0%, 75% 0%, 80% 50%, 75% 100%, 25% 100%, 20% 50%);

    }
</style>
