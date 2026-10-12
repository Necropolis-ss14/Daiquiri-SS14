social-interaction-component-verb = Социальное взаимодействие

pet-verb = Погладить
petting-success = Вы гладите { THE($target) } по {POSS-ADJ($target)} голове.
petting-success-others = { CAPITALIZE(THE($user)) } гладит {THE($target)} по {POSS-ADJ($target)} голове.
petting-emote = гладит {THE($target)} по {POSS-ADJ($target)} голове.

boop-verb = Ткнуть
booping-success = Вы тыкаете { THE($target) } в {POSS-ADJ($target)} нос.
booping-success-others = { CAPITALIZE(THE($user)) } тыкает {THE($target)} в {POSS-ADJ($target)} нос.
booping-emote = тыкает {THE($target)} в {POSS-ADJ($target)} нос.

wave-verb = Помахать
waving-success = Вы машете { THE($target) }.
waving-success-others = { CAPITALIZE(THE($user)) } машет {THE($target)}.
waving-emote = машет {THE($target)}.
waving-emote-self = машет.

look-verb = Посмотреть
looking-success = Вы смотрите на { THE($target) }.
looking-success-others = { CAPITALIZE(THE($user)) } смотрит на {THE($target)}.
looking-emote = смотрит на {THE($target)}.
looking-emote-self = осматривается.

# Sniff - for animals with noses
sniff-verb = Понюхать
sniffing-success = Вы нюхаете { THE($target) }.
sniffing-success-others = {CAPITALIZE(THE($user))} нюхает { THE($target) }.
sniffing-emote = нюхает { THE($target) }.
sniffing-emote-self = нюхаете {REFLEXIVE($target)}.

# Palpate - 'sniffing' but for creatures with antennae
palpate-antennae-verb = Ощупать
palpate-antennae-success = Вы машете антеннами { THE($target) }.
palpate-antennae-success-others = {CAPITALIZE(THE($user))} машет {POSS-ADJ($user)} антеннами { THE($target) }.
palpate-antennae-emote = машет {POSS-ADJ($user)} антеннами { THE($target) }.
palpate-antennae-emote-self = машете {POSS-ADJ($user)} антеннами {REFLEXIVE($target)}.

# Pat
pat-verb = Погладить
patting-success = Вы гладите { THE($target) }.
patting-success-others = {CAPITALIZE(THE($user))} гладит { THE($target) }.
patting-emote = гладит { THE($target) }.

# Boop alt. - Laspi are squishy
booping-laspi-success = Вы тыкаете { THE($target) }. Морда {POSS-ADJ($target)} забавно трясётся.
booping-laspi-success-others = {CAPITALIZE(THE($user))} тыкает { THE($target) }. Морда {POSS-ADJ($target)} забавно трясётся.
booping-laspi-emote = тыкает { THE($target) }. Морда {POSS-ADJ($target)} забавно трясётся.

# Boop alt. - Generic slimes don't have noses
booping-slime-success = Вы тыкаете { THE($target) } туда, где должен быть нос, оставляя вмятину.
booping-slime-success-others = {CAPITALIZE(THE($user))} тыкает { THE($target) } туда, где должен быть нос, оставляя вмятину.
booping-slime-emote = тыкает { THE($target) } туда, где должен быть нос, оставляя вмятину.

# Boop alt. - Cyclorites don't have noses
booping-rocky-success = Вы тыкаете { THE($target) } в каменистый лоб {POSS-ADJ($target)}.
booping-rocky-success-others = {CAPITALIZE(THE($user))} тыкает { THE($target) } в каменистый лоб {POSS-ADJ($target)}.
booping-rocky-emote = тыкает { THE($target) } в каменистый лоб {POSS-ADJ($target)}.

# Boop alt. - Cyborgs and IPCs don't have noses
booping-borg-success = Вы тыкаете { THE($target) } в металлическое лицо {POSS-ADJ($target)}.
booping-borg-success-others = {CAPITALIZE(THE($user))} тыкает { THE($target) } в металлическое лицо {POSS-ADJ($target)}.
booping-borg-emote = тыкает { THE($target) } в металлическое лицо {POSS-ADJ($target)}.

# Boop alt. - AI Cores have screens, not noses
booping-screen-success = Вы тыкаете { THE($target) } в ЭЛТ-экран {POSS-ADJ($target)}.
booping-screen-success-others = {CAPITALIZE(THE($user))} тыкает { THE($target) } в ЭЛТ-экран {POSS-ADJ($target)}.
booping-screen-emote = тыкает { THE($target) } в ЭЛТ-экран {POSS-ADJ($target)}.

# Boop alt. - Parrots and birds have beaks
booping-beak-success = Вы тыкаете { THE($target) } в клюв {POSS-ADJ($target)}.
booping-beak-success-others = {CAPITALIZE(THE($user))} тыкает { THE($target) } в клюв {POSS-ADJ($target)}.
booping-beak-emote = тыкает { THE($target) } в клюв {POSS-ADJ($target)}.

# Boop alt. - Moths don't have noses
booping-moth-success = Вы тыкаете { THE($target) } между сложными глазами {POSS-ADJ($target)}.
booping-moth-success-others = {CAPITALIZE(THE($user))} тыкает { THE($target) } между сложными глазами {POSS-ADJ($target)}.
booping-moth-emote = тыкает { THE($target) } между сложными глазами {POSS-ADJ($target)}.

# Boop alt. - Diona and plants don't have noses
booping-plant-success = Вы тянетесь мимо ветвей { THE($target) } и тыкаете {OBJECT($target)} в кору {POSS-ADJ($target)}.
booping-plant-success-others = {CAPITALIZE(THE($user))} тянется мимо ветвей { THE($target) } и тыкает {OBJECT($target)} в кору {POSS-ADJ($target)}.
booping-plant-emote = тянется мимо ветвей { THE($target) } и тыкает {OBJECT($target)} в кору {POSS-ADJ($target)}.

# Boop alt. - Resomi/Avali and Shadekin don't have noses
booping-snoot-success = Вы тыкаете { THE($target) } в морду {POSS-ADJ($target)}.
booping-snoot-success-others = {CAPITALIZE(THE($user))} тыкает { THE($target) } в морду {POSS-ADJ($target)}.
booping-snoot-emote = тыкает { THE($target) } в морду {POSS-ADJ($target)}.

# Boop alt. - Arachnids and spiders don't have noses
booping-spider-success = Вы тыкаете { THE($target) } между многочисленными глазами {POSS-ADJ($target)}.
booping-spider-success-others = {CAPITALIZE(THE($user))} тыкает { THE($target) } между многочисленными глазами {POSS-ADJ($target)}.
booping-spider-emote = тыкает { THE($target) } между многочисленными глазами {POSS-ADJ($target)}.

# Boop alt. - Tesla (good luck)
booping-tesla-success = Вы неблагоразумно тыкаете { THE($target) } в электрический горизонт событий {POSS-ADJ($target)}.
booping-tesla-success-others = {CAPITALIZE(THE($user))} неблагоразумно тыкает { THE($target) } в электрический горизонт событий {POSS-ADJ($target)}.
booping-tesla-emote = неблагоразумно тыкает { THE($target) } в электрический горизонт событий {POSS-ADJ($target)}.

# Boop alt. - Generic fallback for creatures with no noses.
booping-generic-success = Вы тыкаете { THE($target) } в лицо {POSS-ADJ($target)}.
booping-generic-success-others = {CAPITALIZE(THE($user))} тыкает { THE($target) } в лицо {POSS-ADJ($target)}.
booping-generic-emote = тыкает { THE($target) } в лицо {POSS-ADJ($target)}.
